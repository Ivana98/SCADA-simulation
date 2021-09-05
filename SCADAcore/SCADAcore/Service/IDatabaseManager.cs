using SCADAcore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SCADAcore.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IDatabaseManager" in both code and config file together.
    [ServiceContract(CallbackContract = typeof(IDatabaseManagerCallback))]
    public interface IDatabaseManager
    {
        [OperationContract(IsOneWay = true)]
        void InitService();

        //user operations
        [OperationContract]
        string LogIn(string username, string password);
        [OperationContract]
        bool Register(string username, string password, string role);
        [OperationContract]
        bool DbContainsUser();

        //CRUD operations
        AO GetByIdAO(string id);
        AI GetByIdAI(string id);
        DO GetByIdDO(string id);
        DI GetByIdDI(string id);

        [OperationContract(IsOneWay = true)]
        void DeleteAO(string id);
        [OperationContract(IsOneWay = true)]
        void DeleteDO(string id);
        [OperationContract(IsOneWay = true)]
        void DeleteAI(string id);
        [OperationContract(IsOneWay = true)]
        void DeleteDI(string id);

        [OperationContract(IsOneWay = true)]
        void AddAO(string tagName, string description, string address, double initvalue, double lowlimit, double highlimit);
        [OperationContract(IsOneWay = true)]
        void AddAI(string tagName, string description, string driver, string address, int scantime, bool onoffscan, double lowlimit, double highlimit, string units);
        [OperationContract(IsOneWay = true)]
        void AddDO(string tagName, string description, string address, double initvalue);
        [OperationContract(IsOneWay = true)]
        void AddDI(string tagName, string description, string driver, string address, int scantime, bool onoffscan);

        [OperationContract(IsOneWay = true)]
        void UpdateAO(string tagName, string description, string address, double initvalue, double lowlimit, double highlimit);
        [OperationContract(IsOneWay = true)]
        void UpdateAI(string tagName, string description, string driver, string address, int scantime, bool onoffscan, double lowlimit, double highlimit, string units);
        [OperationContract(IsOneWay = true)]
        void UpdateDO(string tagName, string description, string address, double initvalue);
        [OperationContract(IsOneWay = true)]
        void UpdateDI(string tagName, string description, string driver, string address, int scantime, bool onoffscan);

        [OperationContract]
        string GetAllAO();
        [OperationContract]
        string GetAllAI();
        [OperationContract]
        string GetAllDO();
        [OperationContract]
        string GetAllDI();

        [OperationContract]
        string GetStrAO(string id);
        [OperationContract]
        string GetStrAI(string id);
        [OperationContract]
        string GetStrDO(string id);
        [OperationContract]
        string GetStrDI(string id);

        [OperationContract]
        string GetIdListAO();
        [OperationContract]
        string GetIdListAI();
        [OperationContract]
        string GetIdListDO();
        [OperationContract]
        string GetIdListDI();

        [OperationContract(IsOneWay = true)]
        void TurnScanOnOff(string tagType, string tagId, bool activity);
        [OperationContract(IsOneWay = true)]
        void ChangeValueAO(string tagName, double value);
        [OperationContract(IsOneWay = true)]
        void ChangeValueDO(string tagName, double value);

        [OperationContract(IsOneWay = true)]
        void AddAlarm(string type, double value, string unit, string priority, string tagName);
        [OperationContract(IsOneWay = true)]
        void RemoveAlarm(string tagName, string alarmId);
        [OperationContract]
        string GetAllAlarms(string tagName);

    }

    public interface IDatabaseManagerCallback
    {
        [OperationContract(IsOneWay = true)]
        void WriteToConsole(string message);
    }
}
