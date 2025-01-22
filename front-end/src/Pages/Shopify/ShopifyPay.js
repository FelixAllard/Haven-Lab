// src/shopifyClient.js
import Client from 'shopify-buy';

const client = Client.buildClient({
  domain: 'your-shop-name.myshopify.com', // Replace with your Shopify store domain
  storefrontAccessToken: 'your-access-token', // Replace with your Storefront API Access Token
});

export default client;
