# Dynamics 365 e-Commerce product-collection and data action debugging tips 
Product-collection is the e-commerce module that shows the product list from recommendation service, this docs showcases how to configure this module to show it on e-Commerce site,  and how to debug the underlying code to understand the logic before you want to customize it.

## Configuration Steps for the module
1. Go to site builder, create a new page, choose a template,  and put a generic container in the main slot, and then put a prouct-collection module in it, finally it should like below:
   <img width="1445" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/37f6398b-50b0-458b-a339-fc2561427aa2">
2. Click the "List Style" button to open the dialog:
   <img width="1144" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/91821b34-c0fb-4a7a-be43-b30ed365389a">
3. Choose the editorial list(or any others) and then click next until close the dialog
   <img width="673" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/c8b06916-ffc4-4f53-9578-4903f85cb510">
4. Go to urls, double if it already exists, or you can create a new one:
   <img width="1190" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/ee7c8206-41a0-40f6-9a78-165bbb6d36da">
5.Open the url, and you will see the page shows some products in it:
  <img width="1334" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/cc2b34b5-df0c-42ea-908d-1377c3751d58">

### Debug Tips:
**Important:**  Because e-Commerce utilized the Node-Server Side Rendering to reduce the while-screen time,  so the Typescript code outside SSK modules is not able to debug,  in order to debug these kind of code,  you need make sure:
1. You have the Site Builder permission
2. Append ?setswitch=node_lazyload_all:1 to e-Commerce URL to enable lazy load all.

### Breakpoints can be put(Take Fabrikam as example):
1.	In Product-Collection module:
   <img width="1290" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/07ad29b3-0665-47ae-bbdb-ef13e7fd51a0">
2. Product-list-hydrator:
   <img width="1298" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/05dddade-e6af-44fe-a48d-40a626b6df5c">
3.Products-by-recommendation:
  <img width="1313" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/9fc543a2-2787-4336-a8cf-364c6f4196a5">
4.Products Data Actions:
<img width="1303" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/65ffb2ee-bfdf-4e61-93d0-ad0d32aaf65a">
5.Finally it will go to ProductsController in retail server:
   <img width="761" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/4f5e0634-7012-411c-aab4-2b33b433552f">

### Customization suggestion:
1. Product-collection:
   In the product-collection, after products returns from data action, customer can add logic to customize the products
2. CRT side,  can customize the below request:<br/> 
   <img width="794" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/6d4e28e8-60be-45f8-a601-b423143353c4">




