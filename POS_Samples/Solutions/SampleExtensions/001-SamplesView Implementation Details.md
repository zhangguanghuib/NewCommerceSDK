# Samples View Implementation details:
- Firstly define an interface:
  ```ts
  export interface ISampleItem {
    label: string;
    viewName?: string;
    items?: ISampleItem[];
  }
  ```
  Please notice here viewName and items is nullable, that means some Item only has viewName, but other item only has items, then two kinds of items can resume the same type.
- Secondly define the data:
<img width="549" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/dd05152d-c4ad-4a74-9ae9-a84cf5416563">
From the data you can see:<br/>
  - The first level has label and items
  - The second level has lable and viewName




