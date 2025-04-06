using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Localization
{
    public class Localization : MonoBehaviour
    {
        // prevent multiple call to coroutine if buttons pressed too often
        private bool _active;

        private void Start()
        {
            int id = PlayerPrefs.GetInt("LocaleKey", 0);
            ChangeLocale(id);
        }

        public void ChangeLocale(int localeId)
        {
            if (_active)
                return;
            StartCoroutine(SetLocale(localeId));
        }

        private IEnumerator SetLocale(int localeId)
        {
            _active = true;
            yield return LocalizationSettings.InitializationOperation;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeId];
            PlayerPrefs.SetInt("LocaleKey", localeId);
            _active = false;
        }
    }
}
