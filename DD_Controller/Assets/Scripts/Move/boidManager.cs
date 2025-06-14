using System;
using Monster;
using Shared;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Move
{
    public class BoidsManager : Manager<WalkerDdDealer, WalkerEnum, MonsterVariants>
    {
        [SerializeField] public int bnTargetUpdate = 50;
        private readonly TableList<Rigidbody> _boidsRb = new(0);
        private readonly TableList<Transform> _transform = new(0);
        private TableArray<float> _angleList = new(0, false);
        private TableNArray<float> _sizeList = new(Allocator.Persistent);

        private TableNArray<Vector2> _boidsPos = new(Allocator.Persistent,0, false);

        private TableNArray<int> _groups = new(Allocator.Persistent);
        private TableArray<bool> _hasRb = new(0);

        private int _index;
        private TableArray<int?> _lastTarget = new(0);
        private int _rbSize;

        // Update is called once per frame
        private void FixedUpdate()
        {
            NativeArray<Vector2> posList = _boidsPos.Values;
            float[] angleList = _angleList.Values;
            int rbIndex = 0;
            for (int i = 0; i < Size; ++i)
                if (Active[i])
                {
                    posList[i] =  Vector2Extension.FromV3(_transform[i].position);
                    if (_hasRb[i])
                    {
                        angleList[i] = _boidsRb[rbIndex].rotation.eulerAngles.y;
                        ++rbIndex;
                    }
                    else
                    {
                        angleList[i] = _transform[i].rotation.eulerAngles.y;
                    }
                }
                else if (_hasRb[i])
                {
                    ++rbIndex;
                }

            if (Size != 0)
                _index %= 1 + Size / bnTargetUpdate;

            rbIndex = 0;
            for (int j = 0; j < Size; ++j)
                if (Active[j])
                {
                    Vector2 birdV = posList[j];
                    int? n;
                    float near;

                    if (_lastTarget[j] == null || j % (1 + Size / bnTargetUpdate) == _index)
                    {
                        (n, near) = LookForNearest(j, _groups.Values, posList, Active.Values, _sizeList.Values, Size);
                        if (n == null) return;
                        _lastTarget[j] = n;
                    }
                    else
                    {
                        n = _lastTarget[j];
                        near = (posList[n ?? 0] - birdV).SqrMagnitude();
                    }

                    float newAngle = BoidRuleApply(birdV, angleList[j], near, posList[(int)n], angleList[(int)n],
                        Time.deltaTime * 6, size:2*_sizeList[j]);
                    

                    if (_hasRb[j])
                    {
                        Vector3 rot = _boidsRb[rbIndex].rotation.eulerAngles;
                        rot.y += newAngle;
                        _boidsRb[rbIndex].rotation = Quaternion.Euler(rot);
                        ++rbIndex;
                    }
                    else
                    {
                        Vector3 rot = _transform[j].rotation.eulerAngles;
                        rot.y += newAngle;
                        _transform[j].rotation = Quaternion.Euler(rot);
                    }
                }
                else if (_hasRb[j])
                {
                    ++rbIndex;
                }

            ++_index;
        }

        protected override void AddChunk(int size)
        {
            _transform.AddChunk(size);
            _boidsRb.AddChunk(size);
            _groups.AddChunk(size);
            _lastTarget.AddChunk(size);
            _hasRb.AddChunk(size);
            _boidsPos.AddChunk(size);
            _angleList.AddChunk(size);
            _sizeList.AddChunk(size);
        }

        protected override void InitElement(WalkerDdDealer element)
        {
            Rigidbody rb = element.body;
            if (!rb) return;

            if (_boidsRb.Count == _rbSize)
                _boidsRb.Add(rb);
            else
                _boidsRb[_rbSize] = rb;
            ++_rbSize;
        }

        protected override void RestoreElement(int i, WalkerDdDealer element)
        {
            _groups[i] = element.group;
            _lastTarget[i] = null;
            _sizeList[i] = element.size;
        }

        protected override void AddElementInChunk(WalkerDdDealer element)
        {
            _hasRb[Size] = element.body;
            _transform[Size] = element.transform;
            _lastTarget[Size] = null;
            _groups[Size] = element.group;
            _sizeList[Size] = element.size;
            _boidsPos.Next();
        }

        protected override void AddElementInNew(WalkerDdDealer element)
        {
            _hasRb.Add(element.body);
            _transform.Add(element.transform);
            _boidsPos.Next();
            _angleList.Add();
            _lastTarget.Add();
            _groups.Add(element.group);
            _sizeList.Add(element.size);
        }

        protected override void onEnd()
        {
            _sizeList.End();
            _groups.End();
            _boidsPos.End();
        }

        public void ChangeGroup(WalkerDdDealer element, int? group = null)
        {
            int i = Elements.FindIndex(d => d == element);
            if (i == -1) return;

            _groups[i] = group ?? element.group;
        }
        
        [BurstCompile]
        private static float normal_scalar(Vector2 a, Vector2 b)
        {
            return a.y * b.y - a.x * b.x;
        }
        
        [BurstCompile]
        public static float BoidRuleApply(Vector2 pos, float angle, float dist, Vector2 target, float? targetAngle = null,
            float fact = 1, float rotate = 0, bool charge = false, float size = 1)
        {
            if (targetAngle == null & !charge) throw new ArgumentNullException(nameof(targetAngle));
            float add = rotate * fact;
            float side;
            if (!charge)
                switch (dist/(size*size))
                {
                    case < 4:
                    {
                        side = normal_scalar(
                            new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)), target - pos);
                        if (side > 0)
                            return add + 45 * fact;
                        return add - 45 * fact;
                    }
                    case > 16:
                        side = normal_scalar(
                            new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)), target - pos);
                        if (side < 0)
                            return add + 15 * fact;
                        return add - 15 * fact;
                    default:
                        if (targetAngle > angle)
                            return add + 12 * fact;
                        return add - 12 * fact;
                }
            
            side = normal_scalar(
                new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)), target - pos);
            if (side < 0)
                return add + 15 * fact;
            return add - 15 * fact;
            
        }

        [BurstCompile]
        private static (int?, float) LookForNearest(int current, NativeArray<int> groups, NativeArray<Vector2> targetList, NativeArray<bool> actives, NativeArray<float> boidSize,
            int size)
        {
            Vector2 birdV = targetList[current];
            int g = groups[current];

            Vector2? nearestV = null;
            float near = 0;
            int? i = null;

            for (int k = 0; k < size; ++k)
                if (k != current && actives[k] && (g == 0 || groups[k] == g))
                {
                    Vector2 bV = targetList[k];
                    float dist = (bV - birdV).SqrMagnitude() / (boidSize[k]*boidSize[k]);
                    if (!((nearestV == null) | (near > (bV - birdV).SqrMagnitude()))) continue;
                    nearestV = bV;
                    i = k;
                    near = dist;
                }

            return (i, near);
        }
    }
}