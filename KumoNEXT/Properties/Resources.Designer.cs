﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KumoNEXT.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("KumoNEXT.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to &lt;html&gt;
        ///
        ///&lt;head&gt;
        ///  &lt;meta http-equiv=&quot;Content-Type&quot; content=&quot;text/html; charset=utf-8&quot; /&gt;
        ///  &lt;style&gt;
        ///    /*基础*/
        ///    * {
        ///      user-select: none;
        ///      box-sizing: border-box;
        ///    }
        ///
        ///    body {
        ///      margin: 0px;
        ///    }
        ///
        ///    /*标题栏*/
        ///    .title-bar {
        ///      background-color: white;
        ///      width: 100%;
        ///      height: 28px;
        ///      position: fixed;
        ///      top: 0px;
        ///      left: 0px;
        ///      app-region: drag;
        ///      display: flex;
        ///      flex-direction: row;
        ///      justify-content: space-between;
        ///    } [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Preference {
            get {
                return ResourceManager.GetString("Preference", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;html&gt;
        ///&lt;head&gt;
        ///    &lt;meta http-equiv=&quot;Content-Type&quot; content=&quot;text/html; charset=utf-8&quot; /&gt;
        ///&lt;/head&gt;
        ///&lt;body&gt;
        ///    &lt;script&gt;
        ///        console.log(&quot;Headless Page&quot;);
        ///    &lt;/script&gt;
        ///&lt;/body&gt;
        ///
        ///&lt;/html&gt;.
        /// </summary>
        internal static string PreferenceUpgradeHeadless {
            get {
                return ResourceManager.GetString("PreferenceUpgradeHeadless", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!doctype html&gt;
        ///&lt;html&gt;
        ///
        ///&lt;head&gt;
        ///    &lt;meta charset=&quot;utf-8&quot;&gt;
        ///    &lt;meta http-equiv=&quot;Content-Type&quot; content=&quot;text/html; charset=utf-8&quot;&gt;
        ///    &lt;meta content=&quot;width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0&quot; name=&quot;viewport&quot; /&gt;
        ///    &lt;title&gt;加载失败&lt;/title&gt;
        ///    &lt;style&gt;
        ///        * {
        ///            font-family: &quot;DengXian&quot;, &quot;Microsoft YaHei&quot;;
        ///            box-sizing: border-box;
        ///            user-select: none;
        ///        }
        ///
        ///        .btn {
        ///            background-color: white;
        ///            transi [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string PWA_Error {
            get {
                return ResourceManager.GetString("PWA_Error", resourceCulture);
            }
        }
    }
}
