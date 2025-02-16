import React, { useState, useEffect } from 'react';
import Cookies from 'js-cookie';
import { useTranslation } from 'react-i18next';

const LanguageToggle = () => {
  //default language is english
  const { i18n } = useTranslation();
  const [language, setLanguage] = useState(i18n.language);

  //load saved language
  useEffect(() => {
    const savedLanguage = Cookies.get('language') || 'en';
    setLanguage(savedLanguage);
    i18n.changeLanguage(savedLanguage);
  }, [i18n]);

  const handleLanguageToggle = () => {
    const newLanguage = language === 'en' ? 'fr' : 'en';
    Cookies.set('language', newLanguage, { expires: 365 });
    setLanguage(newLanguage);
    window.location.reload();
  };

  return (
    <button className="btn btn-outline-light" onClick={handleLanguageToggle}>
      {language === 'en' ? 'FR' : 'EN'}
    </button>
  );
};

export default LanguageToggle;
