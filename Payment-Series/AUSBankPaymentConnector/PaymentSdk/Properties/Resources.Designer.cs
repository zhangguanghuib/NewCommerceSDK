﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PaymentSdk.HECConnector.Sample.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("PaymentSdk.HECConnector.Sample.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;form name=&quot;myForm&quot; id=&quot;myForm&quot; target=&quot;Popup_Window&quot; method=&quot;post&quot;
        ///        action=&quot;https://demo.transact.nab.com.au/live/hpp/payment&quot;
        ///        onsubmit=&quot;window.open(&apos;about:blank&apos;,&apos;Popup_Window&apos;,&apos;toolbar=0,scrollbars=0,location=0,statusbar=0,menubar=0,resizable=0,width=800,height=800,left = 312,top = 234&apos;)&quot;&gt;
        ///        &lt;input type=&quot;hidden&quot; name=&quot;vendor_name&quot; value=&quot;DEM0110&quot;&gt;
        ///        &lt;input type=&quot;hidden&quot; name=&quot;payment_alert&quot; value=&quot;test@test.com.au&quot;&gt;
        ///        &lt;input type=&quot;hidden&quot; name=&quot;print_zero_qty&quot; value= [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string PaymentAcceptPageContentsHtml {
            get {
                return ResourceManager.GetString("PaymentAcceptPageContentsHtml", resourceCulture);
            }
        }
    }
}