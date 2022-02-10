using System.Security.Cryptography;
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
    var sha = new SHA256Managed();
    byte[] text = Encoding.UTF8.GetBytes(payload);
    byte[] hash = sha.ComputeHash(text);
    return BitConverter.ToString(hash).Replace("-", "");
  }
}