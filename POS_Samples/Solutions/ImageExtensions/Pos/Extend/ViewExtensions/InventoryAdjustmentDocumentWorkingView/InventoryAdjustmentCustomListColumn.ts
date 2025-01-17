import *  as InventoryAdjustmentDocumentWorkingView from "PosApi/Extend/Views/InventoryAdjustmentDocumentWorkingView";
import { ICustomColumnsContext } from "PosApi/Extend/Views/CustomListColumns";
import { ClientEntities } from "PosApi/Entities";

export default (context: ICustomColumnsContext): InventoryAdjustmentDocumentWorkingView.IInventoryAdjustmentDocumentWorkingAdjustmentCustomListColumn[] => {
    return [
        {
            title: "PRODUCT NUMBER",
            computeValue: (rowData: ClientEntities.IInventoryDocumentLineWithProduct) => { return rowData.product.ItemId; },
            ratio: 15,
            collapseOrder: 1,
            minWidth: 200,
            isRightAligned: true
        },
        {
            title: "PRODUCT",
            computeValue: (rowData: ClientEntities.IInventoryDocumentLineWithProduct) => { return rowData.product.Name; },
            ratio: 20,
            collapseOrder: 2,
            minWidth: 300,
            isRightAligned: true
        },
        {
            title: "LOCATION",
            computeValue: (rowData: ClientEntities.IInventoryDocumentLineWithProduct) => { return ""; },
            ratio: 10,
            collapseOrder: 3,
            minWidth: 100,
            isRightAligned: true
        },
        {
            title: "ADJUSTIN TYPE",
            computeValue: (rowData: ClientEntities.IInventoryDocumentLineWithProduct) => { return rowData.AdjustmentTypeValue +''; },
            ratio: 20,
            collapseOrder: 4,
            minWidth: 100,
            isRightAligned: true
        },
        {
            title: "Quantity",
            computeValue: (rowData: ClientEntities.IInventoryDocumentLineWithProduct) => { return rowData.QuantityOrdered+''; },
            ratio: 15,
            collapseOrder: 5,
            minWidth: 150,
            isRightAligned: true
        },
        {
            title: "UNIT",
            computeValue: (rowData: ClientEntities.IInventoryDocumentLineWithProduct) => { return rowData.UnitOfMeasure; },
            ratio: 10,
            collapseOrder: 6,
            minWidth: 200,
            isRightAligned: true
        },
        {
            title: "Reason",
            computeValue: (rowData: ClientEntities.IInventoryDocumentLineWithProduct) => {
                const documentRecId: number = rowData.SourceDocumentRecordId;
                console.log("documentRecId: " + documentRecId);
                return rowData.UnitOfMeasure;
            },
            ratio: 10,
            collapseOrder: 7,
            minWidth: 200,
            isRightAligned: true
        }
    ]
}