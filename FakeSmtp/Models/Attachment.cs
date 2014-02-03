using System.IO;
using System.Runtime.Serialization;

namespace FakeSmtp.Models
{
	[DataContract]
	public class Attachment
	{
		[DataMember]
		public int Id { get; set; }
		
		[DataMember]
		public string Name { get; set; }
		
		public MemoryStream ContentStream { get; set; }
		
		[DataMember]
		public int Capacity { get; set; }

		[DataMember]
		public string Size { get; set; }

		public void SetSize(int capacity)
		{
			Capacity = capacity;
			
			if (capacity < 1024)
				Size =  capacity + " B";
			else if (capacity < 1024 * 1024)
				Size = (capacity / 1024) + " КB";
			else
				Size = (capacity / (1024 * 1024))  + " MB";
		}

	}
}