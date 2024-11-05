import { IInventoryByStoreListColumn } from "PosApi/Extend/Views/InventoryLookupView";
import { ICustomColumnsContext } from "PosApi/Extend/Views/CustomListColumns";
import { BooleanFormatter } from "PosApi/Consume/Formatters";
import { ProxyEntities } from "PosApi/Entities";

function getItemAvailability(orgUnitAvailability: ProxyEntities.OrgUnitAvailability): ProxyEntities.ItemAvailability {
    "use strict";
    return (orgUnitAvailability && orgUnitAvailability.ItemAvailabilities) ? orgUnitAvailability.ItemAvailabilities[0] : undefined;
}

export default (context: ICustomColumnsContext): IInventoryByStoreListColumn[] => {
    return [
        {
            title: "LOCATION",
            computeValue: (row: ProxyEntities.OrgUnitAvailability): string => {
                return row.OrgUnitLocation.OrgUnitName;
            },
            ratio: 30,
            collapseOrder: 5,
            minWidth: 150
        }, {
            title: "STORE",
            computeValue: (row: ProxyEntities.OrgUnitAvailability): string => {
                const nonStoreWarehouseIdentifier: number = 0; // Non-Store Warehouses do not have a Channel Id.
                let isRetailStore: boolean = row.OrgUnitLocation.ChannelId !== nonStoreWarehouseIdentifier;
                return BooleanFormatter.toYesNo(isRetailStore);
            },
            ratio: 10,
            collapseOrder: 2,
            minWidth: 50
        }, {
            title: "INVENTORY",
            computeValue: (row: ProxyEntities.OrgUnitAvailability): string => {
                let inventoryAvailability: ProxyEntities.ItemAvailability = getItemAvailability(row);
                let quantity: number = (inventoryAvailability && inventoryAvailability.AvailableQuantity) ? inventoryAvailability.AvailableQuantity : 0;
                return quantity.toString();
            },
            ratio: 10,
            collapseOrder: 6,
            minWidth: 60,
            isRightAligned: true
        }, {
            title: "RESERVED",
            computeValue: (row: ProxyEntities.OrgUnitAvailability): string => {
                let inventoryAvailability: ProxyEntities.ItemAvailability = getItemAvailability(row);
                let physicalReserved: number = (inventoryAvailability && inventoryAvailability.PhysicalReserved) ? inventoryAvailability.PhysicalReserved : 0;
                return physicalReserved.toString();
            },
            ratio: 10,
            collapseOrder: 3,
            minWidth: 60,
            isRightAligned: true
        }, {
            title: "ORDERED",
            computeValue: (row: ProxyEntities.OrgUnitAvailability): string => {
                let inventoryAvailability: ProxyEntities.ItemAvailability = getItemAvailability(row);
                let orderedSum: number = (inventoryAvailability && inventoryAvailability.OrderedSum) ? inventoryAvailability.OrderedSum : 0;
                return orderedSum.toString();
            },
            ratio: 10,
            collapseOrder: 4,
            minWidth: 60,
            isRightAligned: true
        }, {
            title: "UNIT",
            computeValue: (row: ProxyEntities.OrgUnitAvailability): string => {
                let inventoryAvailability: ProxyEntities.ItemAvailability = getItemAvailability(row);
                let unitOfMeasure: string = (inventoryAvailability && inventoryAvailability.UnitOfMeasure) ? inventoryAvailability.UnitOfMeasure : "";
                return unitOfMeasure;
            },
            ratio: 10,
            collapseOrder: 1,
            minWidth: 60,
            isRightAligned: true
        },
        {
            title: "Custom1",
            computeValue: (row: ProxyEntities.OrgUnitAvailability): string => {
                let inventoryAvailability: ProxyEntities.ItemAvailability = getItemAvailability(row);
                let commerceProperty: ProxyEntities.CommerceProperty = (inventoryAvailability && inventoryAvailability.ExtensionProperties && inventoryAvailability.ExtensionProperties.length >= 1) ?
                    inventoryAvailability.ExtensionProperties[0] : null;
                let customQty: number = commerceProperty.Value.IntegerValue;   
                return customQty.toString();
            },
            ratio: 10,
            collapseOrder: 7,
            minWidth: 60,
            isRightAligned: true
        },
        {
            title: "Custom2",
            computeValue: (row: ProxyEntities.OrgUnitAvailability): string => {
                let inventoryAvailability: ProxyEntities.ItemAvailability = getItemAvailability(row);
                let commerceProperty: ProxyEntities.CommerceProperty = (inventoryAvailability && inventoryAvailability.ExtensionProperties && inventoryAvailability.ExtensionProperties.length >= 2) ?
                    inventoryAvailability.ExtensionProperties[1] : null;
                let customQty: number = commerceProperty.Value.IntegerValue;
                return customQty.toString();
            },
            ratio: 10,
            collapseOrder: 8,
            minWidth: 60,
            isRightAligned: true
        }
    ];
};