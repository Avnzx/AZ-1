using System;
using Godot;

public class STUNHelper {

  async System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> GetStunServersList() {
    var hclient = new System.Net.Http.HttpClient();
    return await hclient.GetAsync("https://raw.githubusercontent.com/pradt2/always-online-stun/master/valid_hosts.txt");
  }

  async IOption<string> GetBestStunServer() {
    var hclient = new System.Net.Http.HttpClient();
    string file;
    file = await hclient.GetStringAsync("https://raw.githubusercontent.com/pradt2/always-online-stun/master/valid_hosts.txt");
    
    
  }


  byte[] GenerateTransactionID() {
    // endianness doesn’t matter here, it’s all single bytes
    var buffer = new byte[12];
    // TODO: FIX
    // for n in 12:
    //   buffer.append(makerandom(0, 255));
    return buffer;
  }

  byte[] CreateBindingRequestMessage(byte[] transactionId) {
    // endianness matters here, so we need to wrap this
    var buffer = new byte[20];
    var message = new StreamPeerBuffer();
    message.DataArray = buffer;

    // Set the endianness to big endian
    message.BigEndian = true;
    
    // Arrange our STUN header
    message.PutU64(0x0001000000000000);
    
    // Insert our TransactionId
    message.PutData(transactionId);
    return message.DataArray;
  }

  // void GetStunInfo(int port) {
  //   var ppeer = new Godot.PacketPeerUdp();
    
  //   // This must be the port that the host will live on.
  //   ppeer.Bind(port);
    

  //   // Point the packet at the STUN server.
  //   ppeer.ConnectToHost(stunServer.address, int(stunServer.port));


  //   // Build a TransactionId
  //   var transactionId = GenerateTransactionID();
    
  //   // Put it all together, and send it off
  //   var requestMessage = createBindingRequestMessage(transactionId);
  //   ppeer.PutPacket(requestMessage);

  //   // Patiently wait for a reply from the STUN server, without blocking
  //   var timeout = Time.GetTicksMsec() + 5000;
  //   while( ppeer.GetAvailablePacketCount() == 0 ) {
  //     if(Time.GetTicksMsec() > timeout ) return;
  //     // wait on a different thread or something eye dee kay
  //   }
    
  //   var responseMessage = ppeer.GetPacket();
    
  //   // Get the responseType. We're endian-agnostic here, I'll explain below.
  //   // We don't need to wrap this in a stream buffer yet.
  //   var responseType = responseMessage.decode_u16(0);
    
  //   // Nab the TransactionId from the response. Since this is only a collection
  //   // of individual bytes, it's also endian-agnostic.
  //   var responseTransactionId = responseMessage.slice(8, 20);
    
  //   // 0x0101 from a STUN server is a STUN BINDING RESPONSE, 01 01
  //   // is the same no matter which endianness we use, so it's 
  //   // not necessary to wrap this in a stream buffer.
  //   if responseType != 0x0101 or responseTransactionId != transactionId: 
  //     print('Recieved invalid STUN binding response');
  //     return;
      
  //   // Our address data begins at position 24 and onward.
  //   var responseAddress = parseAddressAttributes(responseMessage.slice(24));
    
  //   print('Public IP Address: ', responseAddress.address);
  //   print('Public Port: ', responseAddress.port);
    
  //   // Free up the ppeer, so that the user can host properly.
  //   ppeer.Close();
  // }

  // (System.Net.IPAddress, int) ParseAddressAttributes(byte[] attributes) {

  //   // We care about endianness when reading many of these fields
  //   var streamBuffer = new StreamPeerBuffer();
  //   streamBuffer.DataArray = attributes;
  //   streamBuffer.BigEndian = true;
    
  //   // The address type is up first, either 0x0001 or 0x0002
  //   var addressType = streamBuffer.GetU16();
    
  //   // Then we pull our port out.
  //   var portNumber = streamBuffer.GetU16();
    
  //   // Parse out the IP address
  //   string ipAddress = "";
    
  //   // IPv4 handling
  //   if addressType == 0x01:
  //     var address = [];
  //     for n in 4:
  //       address.push_back(streamBuffer.get_u8());
  //     ipAddress = array_join(address, ".");
  //   // IPv6 handling
  //   elif addressType == 0x02:
  //     const ipv6Segments = [];
  //     for n in 8:
  //   // pull each segment as a u16, and convert it to a hex string representation. and pad it out. The padding might not be necessary, but I'm not an expert on IPv6 display.
  //       ipv6Segments.push_back(
  //         String.num_uint64(
  //           streamBuffer.get_u16(), 
  //           16
  //         )
  //         .lpad(4, "0")
  //       );
  //     ipAddress = array_join(ipv6Segments, ":");
    
  //   // return {
  //   // 	"address": ipAddress,
  //   // 	"port": portNumber
  //   // }
  // }
}