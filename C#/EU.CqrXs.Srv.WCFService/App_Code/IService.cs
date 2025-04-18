using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService" in both code and config file together.
[ServiceContract]
public interface IService
{

	[OperationContract]
	string Send1StSrvMsg(string cryptMsg);

	[OperationContract]
	string ChatRoomInvite(string cryptMsg);


	[OperationContract]
	string ChatRoomPoll(string cryptMsg);


	[OperationContract]
	string ChatRoomPushMessage(string cryptMsg, string chatRoomMembersCrypted);


	[OperationContract]
	string ChatRoomClose(string cryptMsg);


	[OperationContract]
	string TestService();


	[OperationContract]
	string GetIPAddress();


	[OperationContract]
	string TestCache();


    [OperationContract]
	string GetData(int value);


	[OperationContract]
	CompositeType GetDataUsingDataContract(CompositeType composite);

	// TODO: Add your service operations here
}

// Use a data contract as illustrated in the sample below to add composite types to service operations.
[DataContract]
public class CompositeType
{
	bool boolValue = true;
	string stringValue = "Hello ";

	[DataMember]
	public bool BoolValue
	{
		get { return boolValue; }
		set { boolValue = value; }
	}

	[DataMember]
	public string StringValue
	{
		get { return stringValue; }
		set { stringValue = value; }
	}
}
