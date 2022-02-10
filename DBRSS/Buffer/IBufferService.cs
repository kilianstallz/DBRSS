namespace DBRSS.Buffer; 

public interface IBufferService {
  public BufferSendPackage? GetFirstBufferPacket();
  public void AcknowledgeBufferPacket(long packetId);
}