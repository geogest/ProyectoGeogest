﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

// 
// This source code was auto-generated by wsdl, Version=4.6.1055.0.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.1055.0")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Web.Services.WebServiceBindingAttribute(Name = "CrSeedSoapBinding", Namespace = "https://maullin.sii.cl/DTEWS/CrSeed.jws")]
public partial class CrSeedService : System.Web.Services.Protocols.SoapHttpClientProtocol
{

    private System.Threading.SendOrPostCallback getStateOperationCompleted;

    private System.Threading.SendOrPostCallback getSeedOperationCompleted;

    private System.Threading.SendOrPostCallback getVersionPatchOperationCompleted;

    private System.Threading.SendOrPostCallback getVersionMayorOperationCompleted;

    private System.Threading.SendOrPostCallback getVersionMenorOperationCompleted;

    /// <remarks/>
    public CrSeedService()
    {
        this.Url = "https://maullin.sii.cl/DTEWS/CrSeed.jws";
    }

    /// <remarks/>
    public event getStateCompletedEventHandler getStateCompleted;

    /// <remarks/>
    public event getSeedCompletedEventHandler getSeedCompleted;

    /// <remarks/>
    public event getVersionPatchCompletedEventHandler getVersionPatchCompleted;

    /// <remarks/>
    public event getVersionMayorCompletedEventHandler getVersionMayorCompleted;

    /// <remarks/>
    public event getVersionMenorCompletedEventHandler getVersionMenorCompleted;

    /// <remarks/>
    [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "https://maullin.sii.cl/DTEWS/CrSeed.jws", ResponseNamespace = "https://maullin.sii.cl/DTEWS/CrSeed.jws", Use = System.Web.Services.Description.SoapBindingUse.Literal)]
    [return: System.Xml.Serialization.XmlElementAttribute("getStateReturn")]
    public string getState()
    {
        object[] results = this.Invoke("getState", new object[0]);
        return ((string)(results[0]));
    }

    /// <remarks/>
    public System.IAsyncResult BegingetState(System.AsyncCallback callback, object asyncState)
    {
        return this.BeginInvoke("getState", new object[0], callback, asyncState);
    }

    /// <remarks/>
    public string EndgetState(System.IAsyncResult asyncResult)
    {
        object[] results = this.EndInvoke(asyncResult);
        return ((string)(results[0]));
    }

    /// <remarks/>
    public void getStateAsync()
    {
        this.getStateAsync(null);
    }

    /// <remarks/>
    public void getStateAsync(object userState)
    {
        if ((this.getStateOperationCompleted == null))
        {
            this.getStateOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetStateOperationCompleted);
        }
        this.InvokeAsync("getState", new object[0], this.getStateOperationCompleted, userState);
    }

    private void OngetStateOperationCompleted(object arg)
    {
        if ((this.getStateCompleted != null))
        {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.getStateCompleted(this, new getStateCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }

    /// <remarks/>
    [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "https://maullin.sii.cl/DTEWS/CrSeed.jws", ResponseNamespace = "https://maullin.sii.cl/DTEWS/CrSeed.jws", Use = System.Web.Services.Description.SoapBindingUse.Literal)]
    [return: System.Xml.Serialization.XmlElementAttribute("getSeedReturn")]
    public string getSeed()
    {
        object[] results = this.Invoke("getSeed", new object[0]);
        return ((string)(results[0]));
    }

    /// <remarks/>
    public System.IAsyncResult BegingetSeed(System.AsyncCallback callback, object asyncState)
    {
        return this.BeginInvoke("getSeed", new object[0], callback, asyncState);
    }

    /// <remarks/>
    public string EndgetSeed(System.IAsyncResult asyncResult)
    {
        object[] results = this.EndInvoke(asyncResult);
        return ((string)(results[0]));
    }

    /// <remarks/>
    public void getSeedAsync()
    {
        this.getSeedAsync(null);
    }

    /// <remarks/>
    public void getSeedAsync(object userState)
    {
        if ((this.getSeedOperationCompleted == null))
        {
            this.getSeedOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetSeedOperationCompleted);
        }
        this.InvokeAsync("getSeed", new object[0], this.getSeedOperationCompleted, userState);
    }

    private void OngetSeedOperationCompleted(object arg)
    {
        if ((this.getSeedCompleted != null))
        {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.getSeedCompleted(this, new getSeedCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }

    /// <remarks/>
    [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "https://maullin.sii.cl/DTEWS/CrSeed.jws", ResponseNamespace = "https://maullin.sii.cl/DTEWS/CrSeed.jws", Use = System.Web.Services.Description.SoapBindingUse.Literal)]
    [return: System.Xml.Serialization.XmlElementAttribute("getVersionPatchReturn")]
    public string getVersionPatch()
    {
        object[] results = this.Invoke("getVersionPatch", new object[0]);
        return ((string)(results[0]));
    }

    /// <remarks/>
    public System.IAsyncResult BegingetVersionPatch(System.AsyncCallback callback, object asyncState)
    {
        return this.BeginInvoke("getVersionPatch", new object[0], callback, asyncState);
    }

    /// <remarks/>
    public string EndgetVersionPatch(System.IAsyncResult asyncResult)
    {
        object[] results = this.EndInvoke(asyncResult);
        return ((string)(results[0]));
    }

    /// <remarks/>
    public void getVersionPatchAsync()
    {
        this.getVersionPatchAsync(null);
    }

    /// <remarks/>
    public void getVersionPatchAsync(object userState)
    {
        if ((this.getVersionPatchOperationCompleted == null))
        {
            this.getVersionPatchOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetVersionPatchOperationCompleted);
        }
        this.InvokeAsync("getVersionPatch", new object[0], this.getVersionPatchOperationCompleted, userState);
    }

    private void OngetVersionPatchOperationCompleted(object arg)
    {
        if ((this.getVersionPatchCompleted != null))
        {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.getVersionPatchCompleted(this, new getVersionPatchCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }

    /// <remarks/>
    [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "https://maullin.sii.cl/DTEWS/CrSeed.jws", ResponseNamespace = "https://maullin.sii.cl/DTEWS/CrSeed.jws", Use = System.Web.Services.Description.SoapBindingUse.Literal)]
    [return: System.Xml.Serialization.XmlElementAttribute("getVersionMayorReturn")]
    public string getVersionMayor()
    {
        object[] results = this.Invoke("getVersionMayor", new object[0]);
        return ((string)(results[0]));
    }

    /// <remarks/>
    public System.IAsyncResult BegingetVersionMayor(System.AsyncCallback callback, object asyncState)
    {
        return this.BeginInvoke("getVersionMayor", new object[0], callback, asyncState);
    }

    /// <remarks/>
    public string EndgetVersionMayor(System.IAsyncResult asyncResult)
    {
        object[] results = this.EndInvoke(asyncResult);
        return ((string)(results[0]));
    }

    /// <remarks/>
    public void getVersionMayorAsync()
    {
        this.getVersionMayorAsync(null);
    }

    /// <remarks/>
    public void getVersionMayorAsync(object userState)
    {
        if ((this.getVersionMayorOperationCompleted == null))
        {
            this.getVersionMayorOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetVersionMayorOperationCompleted);
        }
        this.InvokeAsync("getVersionMayor", new object[0], this.getVersionMayorOperationCompleted, userState);
    }

    private void OngetVersionMayorOperationCompleted(object arg)
    {
        if ((this.getVersionMayorCompleted != null))
        {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.getVersionMayorCompleted(this, new getVersionMayorCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }

    /// <remarks/>
    [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "https://maullin.sii.cl/DTEWS/CrSeed.jws", ResponseNamespace = "https://maullin.sii.cl/DTEWS/CrSeed.jws", Use = System.Web.Services.Description.SoapBindingUse.Literal)]
    [return: System.Xml.Serialization.XmlElementAttribute("getVersionMenorReturn")]
    public string getVersionMenor()
    {
        object[] results = this.Invoke("getVersionMenor", new object[0]);
        return ((string)(results[0]));
    }

    /// <remarks/>
    public System.IAsyncResult BegingetVersionMenor(System.AsyncCallback callback, object asyncState)
    {
        return this.BeginInvoke("getVersionMenor", new object[0], callback, asyncState);
    }

    /// <remarks/>
    public string EndgetVersionMenor(System.IAsyncResult asyncResult)
    {
        object[] results = this.EndInvoke(asyncResult);
        return ((string)(results[0]));
    }

    /// <remarks/>
    public void getVersionMenorAsync()
    {
        this.getVersionMenorAsync(null);
    }

    /// <remarks/>
    public void getVersionMenorAsync(object userState)
    {
        if ((this.getVersionMenorOperationCompleted == null))
        {
            this.getVersionMenorOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetVersionMenorOperationCompleted);
        }
        this.InvokeAsync("getVersionMenor", new object[0], this.getVersionMenorOperationCompleted, userState);
    }

    private void OngetVersionMenorOperationCompleted(object arg)
    {
        if ((this.getVersionMenorCompleted != null))
        {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.getVersionMenorCompleted(this, new getVersionMenorCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }

    /// <remarks/>
    public new void CancelAsync(object userState)
    {
        base.CancelAsync(userState);
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.1055.0")]
public delegate void getSeedCompletedEventHandler(object sender, getSeedCompletedEventArgs e);

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.1055.0")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class getSeedCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
{

    private object[] results;

    internal getSeedCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
    {
        this.results = results;
    }

    /// <remarks/>
    public string Result
    {
        get
        {
            this.RaiseExceptionIfNecessary();
            return ((string)(this.results[0]));
        }
    }
}




