import javax.crypto.Mac;
import javax.crypto.spec.SecretKeySpec;

public class AuthSignature {

  private static String ALGORITHM = "HmacSHA256";
  private static String ENCODING = "UTF-8";

  /**
   * Generates authentication signature.
   *
   * @param secretKey Secret key.
   * @param nonce Nonce.
   * @param path Request path.
   *
   * @return Authentication signature.
   *
   * @throws Exception when unable to generate the signature.
   */
  public String generate(String secretKey, String nonce, String path)
    throws Exception {
    try {
      Mac mac = Mac.getInstance(AuthSignature.ALGORITHM);
      SecretKeySpec key = new SecretKeySpec(
        secretKey.getBytes(AuthSignature.ENCODING),
        AuthSignature.ALGORITHM
      );
      mac.init(key);
      mac.update(nonce.getBytes(AuthSignature.ENCODING));
      mac.update(path.getBytes(AuthSignature.ENCODING));
      byte[] resultBytes = mac.doFinal();
      StringBuffer hash = new StringBuffer();
      for (int i = 0; i < resultBytes.length; i++) {
        String hex = Integer.toHexString(0xFF & resultBytes[i]);
        if (hex.length() == 1) {
          hash.append('0');
        }
        hash.append(hex);
      }
      return hash.toString();
    } catch (Exception e) {
      throw e;
    }
  }

  public static void main(String[] args) {
    String secretKey = "YOUR_SECRET_KEY";
    // We use UNIX timestamp (in seconds) as nonce value here.
    long unixTime = System.currentTimeMillis() / 1000;
    String nonce = String.valueOf(unixTime);
    String path = "/api/v1/translate";
    AuthSignature authSignature = new AuthSignature();
    try {
      String signature = authSignature.generate(secretKey, nonce, path);
      System.out.println(String.format("Secret key: %s", secretKey));
      System.out.println(String.format("Signature: %s", signature));
      System.out.println(String.format("Nonce: %s", nonce));
      System.out.println(String.format("Path: %s", path));
    } catch (Exception e) {
      System.err.println(e.getMessage());
    }
  }
}
