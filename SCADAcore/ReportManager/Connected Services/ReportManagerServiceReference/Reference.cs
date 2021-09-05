﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ReportManager.ReportManagerServiceReference {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ReportManagerServiceReference.IReportManager")]
    public interface IReportManager {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportManager/AllAlarmsByTime", ReplyAction="http://tempuri.org/IReportManager/AllAlarmsByTimeResponse")]
        string AllAlarmsByTime(System.DateTime startDate, System.DateTime endDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportManager/AllAlarmsByTime", ReplyAction="http://tempuri.org/IReportManager/AllAlarmsByTimeResponse")]
        System.Threading.Tasks.Task<string> AllAlarmsByTimeAsync(System.DateTime startDate, System.DateTime endDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportManager/AllAlarmsByPriority", ReplyAction="http://tempuri.org/IReportManager/AllAlarmsByPriorityResponse")]
        string AllAlarmsByPriority(int priority);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportManager/AllAlarmsByPriority", ReplyAction="http://tempuri.org/IReportManager/AllAlarmsByPriorityResponse")]
        System.Threading.Tasks.Task<string> AllAlarmsByPriorityAsync(int priority);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportManager/AllTagsByTime", ReplyAction="http://tempuri.org/IReportManager/AllTagsByTimeResponse")]
        string AllTagsByTime(System.DateTime startDate, System.DateTime endDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportManager/AllTagsByTime", ReplyAction="http://tempuri.org/IReportManager/AllTagsByTimeResponse")]
        System.Threading.Tasks.Task<string> AllTagsByTimeAsync(System.DateTime startDate, System.DateTime endDate);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportManager/AllAI", ReplyAction="http://tempuri.org/IReportManager/AllAIResponse")]
        string AllAI();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportManager/AllAI", ReplyAction="http://tempuri.org/IReportManager/AllAIResponse")]
        System.Threading.Tasks.Task<string> AllAIAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportManager/AllDI", ReplyAction="http://tempuri.org/IReportManager/AllDIResponse")]
        string AllDI();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportManager/AllDI", ReplyAction="http://tempuri.org/IReportManager/AllDIResponse")]
        System.Threading.Tasks.Task<string> AllDIAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportManager/AllTagById", ReplyAction="http://tempuri.org/IReportManager/AllTagByIdResponse")]
        string AllTagById(string id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IReportManager/AllTagById", ReplyAction="http://tempuri.org/IReportManager/AllTagByIdResponse")]
        System.Threading.Tasks.Task<string> AllTagByIdAsync(string id);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IReportManagerChannel : ReportManager.ReportManagerServiceReference.IReportManager, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ReportManagerClient : System.ServiceModel.ClientBase<ReportManager.ReportManagerServiceReference.IReportManager>, ReportManager.ReportManagerServiceReference.IReportManager {
        
        public ReportManagerClient() {
        }
        
        public ReportManagerClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ReportManagerClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ReportManagerClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ReportManagerClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string AllAlarmsByTime(System.DateTime startDate, System.DateTime endDate) {
            return base.Channel.AllAlarmsByTime(startDate, endDate);
        }
        
        public System.Threading.Tasks.Task<string> AllAlarmsByTimeAsync(System.DateTime startDate, System.DateTime endDate) {
            return base.Channel.AllAlarmsByTimeAsync(startDate, endDate);
        }
        
        public string AllAlarmsByPriority(int priority) {
            return base.Channel.AllAlarmsByPriority(priority);
        }
        
        public System.Threading.Tasks.Task<string> AllAlarmsByPriorityAsync(int priority) {
            return base.Channel.AllAlarmsByPriorityAsync(priority);
        }
        
        public string AllTagsByTime(System.DateTime startDate, System.DateTime endDate) {
            return base.Channel.AllTagsByTime(startDate, endDate);
        }
        
        public System.Threading.Tasks.Task<string> AllTagsByTimeAsync(System.DateTime startDate, System.DateTime endDate) {
            return base.Channel.AllTagsByTimeAsync(startDate, endDate);
        }
        
        public string AllAI() {
            return base.Channel.AllAI();
        }
        
        public System.Threading.Tasks.Task<string> AllAIAsync() {
            return base.Channel.AllAIAsync();
        }
        
        public string AllDI() {
            return base.Channel.AllDI();
        }
        
        public System.Threading.Tasks.Task<string> AllDIAsync() {
            return base.Channel.AllDIAsync();
        }
        
        public string AllTagById(string id) {
            return base.Channel.AllTagById(id);
        }
        
        public System.Threading.Tasks.Task<string> AllTagByIdAsync(string id) {
            return base.Channel.AllTagByIdAsync(id);
        }
    }
}
