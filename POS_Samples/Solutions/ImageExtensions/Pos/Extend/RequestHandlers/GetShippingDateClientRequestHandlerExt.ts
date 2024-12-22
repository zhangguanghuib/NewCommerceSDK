import { ClientEntities } from "PosApi/Entities";
import { GetShippingDateClientRequestHandler } from "PosApi/Extend/RequestHandlers/CartRequestHandlers";
import { GetShippingDateClientRequest, GetShippingDateClientResponse } from "PosApi/Consume/Cart";
import { ObjectExtensions } from "PosApi/TypeExtensions";
import { Entities } from "../../DataService/DataServiceEntities.g";
import { DlvModeBookSlot } from "../../DataService/DataServiceRequests.g";

export default class GetShippingDateClientRequestHandlerExt extends GetShippingDateClientRequestHandler {

    private shippingMethodBookingSlotId: string = 'shippingMethodBookingSlot___';
    private bookSLots: Entities.DlvModeBookSlot[];
    /**
     * Executes the request handler asynchronously.
     * @param {GetShippingDateClientRequest<GetShippingDateClientResponse>} The request containing the response.
     * @return {Promise<ICancelableDataResult<GetShippingDateClientResponse>>} The cancelable promise containing the response.
     */
    public executeAsync(request: GetShippingDateClientRequest<GetShippingDateClientResponse>):
        Promise<ClientEntities.ICancelableDataResult<GetShippingDateClientResponse>> {

        let shippingMethodDiv: HTMLDivElement = document.querySelector('div.section[aria-label="Shipping method"][data-bind*="sectionWrapper: { headerResx: \'string_2504\' }"]');
        console.log(shippingMethodDiv);
        let shippingMethodsSection: HTMLDivElement = shippingMethodDiv.querySelector(`div:nth-child(2)`) as HTMLDivElement;
        console.log(shippingMethodsSection);

        return this.context.runtime.executeAsync(new DlvModeBookSlot.GetDlvModeBookSlotsRequest<DlvModeBookSlot.GetDlvModeBookSlotsResponse>(request.deliveryMethod.Code))
            .then((result: ClientEntities.ICancelableDataResult<DlvModeBookSlot.GetDlvModeBookSlotsResponse>): Promise<void> => {
                this.bookSLots = result.data.result;
                console.log(this.bookSLots);
                // Build your table here
                return Promise.resolve();
            }).catch((reason: any) => {
                this.context.logger.logError("ShippingMethods: " + JSON.stringify(reason));
            }).then(() => {
                let testTable = this.generateTable(this.bookSLots);
                let shippingMethodsList = shippingMethodsSection.querySelector(`div:nth-child(2)`) as HTMLDivElement;;
                shippingMethodsList.appendChild(testTable);
                return this.defaultExecuteAsync(request);
            });
    }

    public generateTable(data: Entities.DlvModeBookSlot[]): HTMLDivElement {
        let shippingMethodBookingSlot: HTMLDivElement = document.getElementById(this.shippingMethodBookingSlotId) as HTMLDivElement;
        if (!ObjectExtensions.isNullOrUndefined(shippingMethodBookingSlot)) {
            shippingMethodBookingSlot.remove();
        }
        shippingMethodBookingSlot = document.createElement('div') as HTMLDivElement;
        shippingMethodBookingSlot.id = this.shippingMethodBookingSlotId;

        const table: HTMLTableElement = document.createElement('table') as HTMLTableElement;

        const thead = document.createElement('thead');
        const headerRow = document.createElement('tr');
        headerRow.style.background = "blue";

        const headers = ['DlvModeCode', 'DlvModeTxt', 'ShippingDate', 'MaxSlot', 'FreeSlot'];
        headers.forEach(headerText => {
            const th = document.createElement('th');
            th.textContent = headerText;
            headerRow.appendChild(th);
        });

        thead.appendChild(headerRow);
        table.appendChild(thead);

        const tbody = document.createElement('tbody');
        data.forEach((slot, index) => {
            const row = document.createElement('tr');

            const cells = [
                slot.DlvModeCode,
                slot.DlvModeTxt,
                slot.ShippingDate.toDateString(),
                slot.MaxSlot.toString(),
                slot.FreeSlot.toString()
            ];

            cells.forEach(cellText => {
                const td = document.createElement('td');
                td.textContent = cellText;
                row.appendChild(td);
            });

            // Apply background color for odd and even rows
            if (index % 2 === 0) {
                row.style.backgroundColor = 'green';
            } else {
                row.style.backgroundColor = '#501669';
            }

            tbody.appendChild(row);
        });

        table.appendChild(tbody);

        // Style the table
        table.style.width = '100%';
        table.style.borderCollapse = 'collapse';
        table.style.fontFamily = 'Arial, sans-serif';
        table.style.fontSize = '14px';
        table.style.color = 'white';
        table.style.border = '2px solid #dddddd';
        table.style.textAlign = 'left';
        table.style.marginTop = '5px';

        // Style the table header
        var headersCells = table.getElementsByTagName('th');
        for (let i = 0; i < headersCells.length; i++) {
            headersCells[i].style.border = '2px solid yellow';
            headersCells[i].style.padding = '8px';
        }

        // Style the table cells
        var cells = table.getElementsByTagName('td');
        for (let k = 0; k < cells.length; k++) {
            cells[k].style.border = '1px solid #dddddd';
            cells[k].style.padding = '8px';
        }

        shippingMethodBookingSlot.appendChild(table);

        return shippingMethodBookingSlot;
    }
}
