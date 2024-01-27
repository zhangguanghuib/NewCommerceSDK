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
- Secondly define th



