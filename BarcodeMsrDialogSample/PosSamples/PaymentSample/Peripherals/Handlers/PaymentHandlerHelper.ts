/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { ObjectExtensions, StringExtensions } from "PosApi/TypeExtensions";

/**
 * Override request handler class for the payment terminal authorize payment request.
 */
export class PaymentHandlerHelper {
    // Get extra properties for payment terminal
    public static FillExtensionProperties(
        cart: ProxyEntities.Cart,
        extensionProperties: ClientEntities.IExtensionTransaction): ClientEntities.IExtensionTransaction {

        let extraProperties: ClientEntities.IExtensionTransaction = null;
        // Build extra extension properties.
        if (!ObjectExtensions.isNullOrUndefined(cart)) {
            extraProperties = {
                ExtensionProperties: [
                    <ProxyEntities.CommerceProperty>{
                        Key: "CartId",
                        Value: <ProxyEntities.CommercePropertyValue>{
                            StringValue: !ObjectExtensions.isNullOrUndefined(cart) ? cart.Id : ""
                        }
                    }, {
                        Key: "ChannelId",
                        Value: <ProxyEntities.CommercePropertyValue>{
                            StringValue: (ObjectExtensions.isNullOrUndefined(cart.ChannelId)) ? "" : cart.ChannelId.toString()
                        }
                    }, {
                        Key: "TerminalId",
                        Value: <ProxyEntities.CommercePropertyValue>{
                            StringValue: cart.TerminalId
                        }
                    }, {
                        Key: "StaffId",
                        Value: <ProxyEntities.CommercePropertyValue>{ StringValue: cart.StaffId }
                    }, {
                        Key: "CustomerId",
                        Value: <ProxyEntities.CommercePropertyValue>{
                            StringValue: (StringExtensions.isNullOrWhitespace(cart.CustomerId)) ? "" : cart.CustomerId
                        }
                    }, {
                        Key: "ShippingZipCode",
                        Value: <ProxyEntities.CommercePropertyValue>{
                            StringValue: !ObjectExtensions.isNullOrUndefined(cart.ShippingAddress) ?
                                (StringExtensions.isNullOrWhitespace(cart.ShippingAddress.ZipCode) ? "" : cart.ShippingAddress.ZipCode) : ""
                        }
                    }
                ]
            };
        }

        if (ObjectExtensions.isNullOrUndefined(extensionProperties)) {
            extensionProperties = extraProperties;
        } else {
            for (let i: number = 0; i < extraProperties.ExtensionProperties.length; i++) {
                extensionProperties.ExtensionProperties.push(extraProperties.ExtensionProperties[i]);
            }
        }

        return extensionProperties;
    }
}