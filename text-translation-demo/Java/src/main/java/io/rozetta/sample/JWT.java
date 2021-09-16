package io.rozetta.sample;

import java.io.StringReader;
import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.time.Duration;

import jakarta.json.Json;

class JWT {
  private String accessKey = null;

  private String secretKey = null;

  private int duration = 0;

  private HttpClient httpClient = null;

  /**
   * JSON Web Token utility.
   * 
   * @param accessKey Signans API access key.
   * @param secretKey Signans API secret key.
   * @param duration  Valid period of the retrieved JWT, in number of seconds.
   */
  public JWT(String accessKey, String secretKey, int duration) {
    this.accessKey = accessKey;
    this.secretKey = secretKey;
    this.duration = duration;
    this.httpClient = HttpClient.newBuilder()
        .version(HttpClient.Version.HTTP_2)
        .connectTimeout(Duration.ofSeconds(API.CONNECT_TIMEOUT))
        .build();
  }

  public String getToken() throws Exception {
    String data = Json.createObjectBuilder()
        .add("accessKey", this.accessKey)
        .add("secretKey", this.secretKey)
        .add("duration", this.duration)
        .build()
        .toString();
    String url = API.BASE_URL + "/api/v1/token";
    HttpRequest request = HttpRequest.newBuilder()
        .POST(HttpRequest.BodyPublishers.ofString(data))
        .uri(URI.create(url))
        .header("Content-Type", "application/json")
        .build();
    HttpResponse<String> response = this.httpClient.send(request, HttpResponse.BodyHandlers.ofString());
    if ((response.statusCode() % 200) != 0) {
      throw new Exception("Unable to get JWT: " + response.body());
    }
    String jwt = Json.createReader(new StringReader(response.body()))
        .readObject()
        .getJsonObject("data")
        .getString("encodedJWT");
    return jwt;
  }

  public static void main(String[] args) {
    JWT jwt = new JWT(API.ACCESS_KEY, API.SECRET_KEY, API.JWT_DURATION);
    try {
      String token = jwt.getToken();
      System.out.println("JWT: " + token);
    } catch (Exception e) {
      e.printStackTrace();
    }
  }
}
