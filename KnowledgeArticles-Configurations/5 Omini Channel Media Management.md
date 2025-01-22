0. Official document:
     https://learn.microsoft.com/en-us/dynamics365/commerce/dev-itpro/omnichannel-media-management-overview <br/>
1:  Setup:<br/>
     ![image](https://github.com/user-attachments/assets/02ce4ebe-aaac-4054-9984-4ee62a16ee76)<br/>
     ![image](https://github.com/user-attachments/assets/9083f6f3-46b7-48d7-8aa4-e165afcecb6b)<br/>
     ![image](https://github.com/user-attachments/assets/322e6567-d758-471d-a9b1-ff702873f1b6)<br/>

2： Demo:
    Site builder:<br/>
    ![image](https://github.com/user-attachments/assets/f5aee725-9ccf-4d7e-9dec-9ef21fe2b7ee)<br/>
    <img width="846" alt="image" src="https://github.com/user-attachments/assets/e555eeaf-091e-4ae6-b8ba-faf4b1df3155" /><br/>
    ![image](https://github.com/user-attachments/assets/38490ab7-1aa0-4572-9331-5365310eb320)<br/>

2.5:  Go to Commerce Site builder:

    Go to each commerce site, and enable<br/>
    ![image](https://github.com/user-attachments/assets/b30d8008-c59b-4aff-a516-7c420e7a89a3)<br/>
    ![image](https://github.com/user-attachments/assets/d81c8a5b-0517-49de-8a70-959ce9496c6f)<br/>
    ![image](https://github.com/user-attachments/assets/d984994f-faa2-47d9-816d-be1ebf3aad80)<br/>
    ![image](https://github.com/user-attachments/assets/27e1e8a9-3d03-480c-a4e0-91432bd2288d)<br/>

3.Omni-Channel Content and product image assignment<br/>
  <img width="681" alt="image" src="https://github.com/user-attachments/assets/5ed5270d-2c82-4b9b-9ade-cf517591f225" /><br/>
  ![image](https://github.com/user-attachments/assets/9ff1da04-d011-4719-a86d-3df871f41ef4)<br/>
  ![image](https://github.com/user-attachments/assets/ff7c8c93-6a83-41c9-805e-26ae160f38bf)<br/>

4.  In Sitebuilder, we assign an image to a product:
    ![image](https://github.com/user-attachments/assets/31826628-59d3-4878-901b-f44bc3e4ff82)<br/>
    in HQ, run this batch job:<br/>
    ![image](https://github.com/user-attachments/assets/bd31fdf8-e189-4e71-a931-e98bb0363690)<br/>

     Then run 1040, see this table got changed:<br/>

    ![image](https://github.com/user-attachments/assets/be889949-ffb4-4d34-a7bc-1b94e581fd83)<br/>
    ax.RETAILMEDIAPRODUCTRELATION<br/>
    ax.RETAILMEDIARESOURCE<br/>
    ax.RETAILMEDIARESOURCETRANSLATION<br/>

5. Wait the cache to refresh.
   Each image has a public url for 3rd-Party system
   <img width="681" alt="image" src="https://github.com/user-attachments/assets/eddf07c3-cd13-46e7-bb9f-796f8cb486f5" /><br/>
   ![image](https://github.com/user-attachments/assets/d5d2bbe4-b2ef-4f95-a3e1-39481de3d77c)<br/>






















