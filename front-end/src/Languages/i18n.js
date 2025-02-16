import i18n from 'i18next';
import {initReactI18next} from 'react-i18next';

import home_en from './en/home.json';
import home_fr from './fr/home.json';
import navbar_en from './en/navbar.json';
import navbar_fr from './fr/navbar.json';
import footer_en from './en/footer.json';
import footer_fr from './fr/footer.json';
import products_en from './en/products.json';
import products_fr from './fr/products.json';
import productpage_en from './en/productpage.json';
import productpage_fr from './fr/productpage.json';
import cart_en from './en/cart.json';
import cart_fr from './fr/cart.json';
import aboutus_en from './en/aboutus.json';
import aboutus_fr from './fr/aboutus.json';

i18n.use(initReactI18next).init({

  debug: true,
  fallbackLng: 'en',

  resources: {
    en: {
      navbar: navbar_en,
      home: home_en,
      footer: footer_en,
      cart: cart_en,
      aboutus: aboutus_en,
      products: products_en,
      productpage: productpage_en
    },

    fr: {
      navbar: navbar_fr,
      home: home_fr,
      footer: footer_fr,
      cart: cart_fr,
      aboutus: aboutus_fr,
      products: products_fr,
      productpage: productpage_fr
    }
  }
});

export default i18n;
