This Sample is to show how to customize form data source to disable some fields.

![image](https://user-images.githubusercontent.com/14832260/219289983-e953c9c8-5fa7-4400-8bf4-0aeda342065b.png)

```c
[ExtensionOf(formdatasourcestr(SalesTable, MCRSalesLine))]
public final class SalesTable_AppSuiteExt_Extension
{
    public void init()
    {

        FormDataSource mcrSalesLine_ds = this;

        next init();

        mcrSalesLine_ds.object(fieldNum(mcrSalesLine, RetailPriceOverrideWorkflowState)).allowEdit(false);
        mcrSalesLine_ds.object(fieldNum(mcrSalesLine, RetailPriceOverrideWorkflowState)).enabled(false);
    }

}
```
