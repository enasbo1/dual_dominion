using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Localization
{
    public class Localization : MonoBehaviour
    {
        public TMP_Dropdown dropdownLang;
        
        private enum LocalizationName
        {
            En,
            Fr,
        }
        
        private static readonly Dictionary<LocalizationName, string> LocalizationNames = new Dictionary<LocalizationName, string>
        {
            { LocalizationName.En, "English" },
            { LocalizationName.Fr, "Français" },
        };
        
        // prevent multiple call to coroutine if buttons pressed too often
        private bool _active;

        private void Start()
        {
            int id = PlayerPrefs.GetInt("LocaleKey", 0);
            LocalizationName localizationName = (LocalizationName)id;
            ChangeLocale(localizationName);
            
            InitLanguageDropdown();
        }
        
        private void InitLanguageDropdown()
        {
            dropdownLang.ClearOptions();

            List<string> options = new List<string>();
            foreach (LocalizationName locale in Enum.GetValues(typeof(LocalizationName)))
            {
                options.Add(LocalizationNames[locale]);
            }

            dropdownLang.AddOptions(options);
            dropdownLang.onValueChanged.AddListener(ChangeLocaleFromInt);
        }

        private void ChangeLocale(LocalizationName localizationToLoad)
        {
            if (_active) return;
            StartCoroutine(SetLocale(localizationToLoad));
        }
        
        private void ChangeLocaleFromInt(int id)
        {
            if (_active) return;
            StartCoroutine(SetLocale((LocalizationName)id));
        }

        private IEnumerator SetLocale(LocalizationName localizationToLoad)
        {
            _active = true;
            yield return LocalizationSettings.InitializationOperation;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[(int)localizationToLoad];
            PlayerPrefs.SetInt("LocaleKey", (int)localizationToLoad);
            _active = false;
        }
    }
}