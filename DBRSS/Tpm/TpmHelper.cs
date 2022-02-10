using System.Text;
using DBRSS.Events;
using Microsoft.Azure.Devices.Provisioning.Security;
using Tpm2Lib;

namespace DBRSS.Tpm; 

public static class TpmHelper {

  public static string GetPublicEk() {
    var provider = new SecurityProviderTpmHsm(null);
    var ek = provider.GetEndorsementKey();
    var ekStr = BitConverter.ToString(ek).Replace("-", "");
    return ekStr;
  }

  public static string HashString(string payload) {
    Tpm2Device tpm2Device = new TbsDevice();
    tpm2Device.Connect();
    var security = new SecurityProviderTpmHsm(null);
    var tpm = new Tpm2(tpm2Device);
    byte[] hash = tpm.Hash(Encoding.ASCII.GetBytes(payload), TpmAlgId.Sha256, TpmHandle.RhOwner, out TkHashcheck validation);
    return BitConverter.ToString(hash).Replace("-", "");
  }
}