import React, { useState, useEffect } from 'react';
import Cookies from 'js-cookie';

const LanguageToggle = () => {
  //default language is english
  const [language, setLanguage] = useState('en');

  //load saved language
  useEffect(() => {
    const savedLanguage = Cookies.get('language') || 'en';
    setLanguage(savedLanguage);
  }, []);

  const handleLanguageToggle = () => {
    const newLanguage = language === 'en' ? 'fr' : 'en';
    setLanguage(newLanguage);
    Cookies.set('language', newLanguage, { expires: 365 });
    window.location.reload();
  };

  return (
    <button className="btn btn-outline-light" onClick={handleLanguageToggle}>
      {language === 'en' ? 'FR' : 'EN'}
    </button>
  );
};

export default LanguageToggle;
