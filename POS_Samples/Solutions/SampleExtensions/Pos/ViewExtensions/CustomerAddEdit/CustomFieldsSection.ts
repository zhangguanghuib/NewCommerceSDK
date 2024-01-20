import ko from "knockout";

import {
    CustomerAddEditCustomControlBase,
    ICustomerAddEditCustomControlState,
    ICustomerAddEditCustomControlContext,
    CustomerAddEditCustomerUpdatedData
} from "PosApi/Extend/Views/CustomerAddEditView";
import { ObjectExtensions, StringExtensions } from "PosApi/TypeExtensions";
import { ProxyEntities } from "PosApi/Entities";
import * as Controls from "PosApi/Consume/Controls";

export default class CustomFieldsSection extends CustomerAddEditCustomControlBase {
    public ssn: ko.Observable<string>;
    public organizationId: ko.Observable<string>;
    public isVip: boolean;
    public customerIsPerson: ko.Observable<boolean>;
    public toggleSwitch: Controls.IToggle;
    private static readonly TEMPLATE_ID: string = "Microsoft_Pos_Extensibility_Samples_CustomFieldsSection";

    constructor(id: string, context: ICustomerAddEditCustomControlContext) {
        super(id, context);

        this.ssn = ko.observable("");
        this.organizationId = ko.observable("");
        this.isVip = false;
        this.customerIsPerson = ko.observable(false);

        this.ssn.subscribe((newValue: string): void => {
            this._addOrUpdateExtensionProperty("ssn", <ProxyEntities.CommercePropertyValue>{ StringValue: newValue });
        });

        this.organizationId.subscribe((newValue: string): void => {
            this._addOrUpdateExtensionProperty("organizationId", <ProxyEntities.CommercePropertyValue>{ StringValue: newValue });
        });

        this.customerUpdatedHandler = (data: CustomerAddEditCustomerUpdatedData) => {
            this.customerIsPerson(data.customer.CustomerTypeValue === ProxyEntities.CustomerType.Person);

            let attributeVal: string = this._getRetailAttributeValue("LoyaltyCustomer1", data.customer);
            console.log(attributeVal);

            let localCust: ProxyEntities.Customer = this.customer;
            console.log(localCust);
        };
    }

    /**
     * Binds the control to the specified element.
     * @param {HTMLElement} element The element to which the control should be bound.
     */
    public onReady(element: HTMLElement): void {
        ko.applyBindingsToNode(element, {
            template: {
                name: CustomFieldsSection.TEMPLATE_ID,
                data: this
            }
        });
        let toggleOptions: Controls.IToggleOptions = {
            labelOn: this.context.resources.getString("string_1357"),
            labelOff: this.context.resources.getString("string_1358"),
            checked: this.isVip,
            enabled: true,
            tabIndex: 0
        };

        let toggleRootElem: HTMLDivElement = element.querySelector("#isVipToggle") as HTMLDivElement;
        this.toggleSwitch = this.context.controlFactory.create(this.context.logger.getNewCorrelationId(), "Toggle", toggleOptions, toggleRootElem);
        this.toggleSwitch.addEventListener("CheckedChanged", (eventData: { checked: boolean }) => {
            this.toggleVip(eventData.checked);
        });

    }

    /**
     * Initializes the control.
     * @param {ICustomerDetailCustomControlState} state The initial state of the page used to initialize the control.
     */
    public init(state: ICustomerAddEditCustomControlState): void {
        if (!state.isSelectionMode) {
            this.isVisible = true;
            this.customerIsPerson(state.customer.CustomerTypeValue === ProxyEntities.CustomerType.Person);

            let attributeVal: string = this._getRetailAttributeValue("LoyaltyCustomer1", state.customer);

            console.log(attributeVal);
        }
    }

    /**
     * Toggles Vip property.
     * @param {boolean} checked Indicates if vip is checked or not.
     */
    public toggleVip(checked: boolean): void {
        this._addOrUpdateExtensionProperty("isVip", <ProxyEntities.CommercePropertyValue>{ BooleanValue: checked });
    }

    /**
     * Gets the property value from the property bag, by its key. Optionally creates the property value on the bag, if it does not exist.
     */
    private _addOrUpdateExtensionProperty(key: string, newValue: ProxyEntities.CommercePropertyValue): void {
        let customer: ProxyEntities.Customer = this.customer;

        let extensionProperty: ProxyEntities.CommerceProperty =
            Commerce.ArrayExtensions.firstOrUndefined(customer.ExtensionProperties, (property: ProxyEntities.CommerceProperty) => {
                return property.Key === key;
            });

        if (ObjectExtensions.isNullOrUndefined(extensionProperty)) {
            let newProperty: ProxyEntities.CommerceProperty = {
                Key: key,
                Value: newValue
            };

            if (ObjectExtensions.isNullOrUndefined(customer.ExtensionProperties)) {
                customer.ExtensionProperties = [];
            }

            customer.ExtensionProperties.push(newProperty);
        } else {
            extensionProperty.Value = newValue;
        }

        this.customer = customer;
    }

    private _getRetailAttributeValue(name: string, customer1?: ProxyEntities.Customer): string {
        let attributeValue: string = StringExtensions.EMPTY;
        if (ObjectExtensions.isNullOrUndefined(this.customer)) {
            return attributeValue;
        }

        //let customer: ProxyEntities.Customer = ObjectExtensions.isNullOrUndefined(customer1)? this.customer: customer1;
        let customer: ProxyEntities.Customer = this.customer;

        if (ObjectExtensions.isNullOrUndefined(customer.Attributes) || customer.Attributes.length === 0) {
            return attributeValue;
        }

        let attributeProperty: ProxyEntities.CustomerAttribute =
            Commerce.ArrayExtensions.firstOrUndefined(customer.Attributes, (property: ProxyEntities.CustomerAttribute) => {
                return property.Name === name;
            });

        if (!ObjectExtensions.isNullOrUndefined(attributeProperty)) {
            attributeValue = attributeProperty.AttributeValue.StringValue;
        }

        return attributeValue;
    }
}