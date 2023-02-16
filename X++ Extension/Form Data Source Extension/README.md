This Sample is to show how to customize form data source to disable some fields.
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
