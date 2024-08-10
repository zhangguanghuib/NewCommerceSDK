import { ProxyEntities } from "PosApi/Entities";
import { ICustomColumnsContext } from "PosApi/Extend/Views/CustomListColumns";
import { IInventoryByStoreListColumn } from "PosApi/Extend/Views/InventoryLookupView";
import { BooleanFormatter } from "PosApi/Consume/Formatters";

function getItemAvailability(orgUnitAvailability: ProxyEntities.OrgUnitAvailability): ProxyEntities.ItemAvailability {
    "use strict";
    return (orgUnitAvailability && orgUnitAvailability.ItemAvailabilities && orgUnitAvailability.ItemAvailabilities.length > 0)
        ? orgUnitAvailability.ItemAvailabilities[0] : undefined;
}

export default (context: ICustomColumnsContext): IInventoryByStoreListColumn[] => {
    return [
        {
            title: "Location_Customized",
            computeValue: (row: ProxyEntities.OrgUnitAvailability): string => {
                return row.OrgUnitLocation.OrgUnitName;
            },
            ratio: 50,
            collapseOrder: 5,
            minWidth: 150
        }, {
            title: "Store",
            computeValue: (row: ProxyEntities.OrgUnitAvailability): string => {
                const nonStoreWarehouseIdentifier: number = 0;
                let isRetailStore: boolean = row.OrgUnitLocation.ChannelId !== nonStoreWarehouseIdentifier;
                return BooleanFormatter.toYesNo(isRetailStore); 
            },
            ratio: 10,
            collapseOrder: 2,
            minWidth: 50
        }, {
            title: "Inventory",
            computeValue: (row: ProxyEntities.OrgUnitAvailability): string => {
                let inventoryAvailabbility: ProxyEntities.ItemAvailability = getItemAvailability(row);
                let quantity: number = (inventoryAvailabbility && inventoryAvailabbility.AvailableQuantity)
                    ? inventoryAvailabbility.AvailableQuantity : 0;
                return quantity.toString();
            },
            ratio: 10,
            collapseOrder: 6,
            minWidth: 60,
            isRightAligned: true
        }, {
            title: "Reserved",
            computeValue: (row: ProxyEntities.OrgUnitAvailability): string => {
                let inventoryAvailabbility: ProxyEntities.ItemAvailability = getItemAvailability(row);
                let physicalReservered: number = (inventoryAvailabbility && inventoryAvailabbility.PhysicalReserved)
                    ? inventoryAvailabbility.PhysicalReserved : 0;
                return physicalReservered.toString();
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
                let unitofMeasure: string = (inventoryAvailability && inventoryAvailability.UnitOfMeasure) ? inventoryAvailability.UnitOfMeasure : "";
                return unitofMeasure;
            },
            ratio: 10,
            collapseOrder: 1,
            minWidth: 60,
            isRightAligned: true
        }
    ]

}