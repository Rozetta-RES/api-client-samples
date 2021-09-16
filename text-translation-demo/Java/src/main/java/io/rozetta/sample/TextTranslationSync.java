package io.rozetta.sample;

import java.io.StringReader;
import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.time.Duration;

import jakarta.json.Json;
import jakarta.json.JsonArray;
import jakarta.json.JsonObject;
import jakarta.json.JsonValue;

class TextTranslationSync {
  public static void main(String[] args) {
    JWT jwt = new JWT(API.ACCESS_KEY, API.SECRET_KEY, API.JWT_DURATION);
    try {
      String token = jwt.getToken();
      System.out.println(token);
      JsonArray jsonArray = Json.createArrayBuilder()
          .add("Hello.")
          .add("Translate from English to Japanese.")
          .build();
      String data = Json.createObjectBuilder()
          .add("text", jsonArray)
          .add("fieldId", "1")
          .add("sourceLang", "en")
          .add("targetLang", "ja")
          .build()
          .toString();
      String url = API.BASE_URL + "/api/v1/translate";
      System.out.println(data);
      HttpRequest request = HttpRequest.newBuilder()
          .POST(HttpRequest.BodyPublishers.ofString(data))
          .uri(URI.create(url))
          .header("Content-Type", "application/json")
          .header("Authorization", "Bearer " + token)
          .build();
      HttpClient httpClient = HttpClient.newBuilder()
          .version(HttpClient.Version.HTTP_2)
          .connectTimeout(Duration.ofSeconds(API.CONNECT_TIMEOUT))
          .build();
      HttpResponse<String> response = httpClient.send(request, HttpResponse.BodyHandlers.ofString());
      if (response.statusCode() != 200) {
        throw new Exception("Unable to translate text: " + response.body());
      }
      JsonArray results = Json.createReader(new StringReader(response.body()))
          .readObject()
          .getJsonObject("data")
          .getJsonArray("translationResult");
      for (JsonValue jsonValue : results) {
        JsonObject result = jsonValue.asJsonObject();
        System.out.println("Original text: " + result.getString("sourceText"));
        System.out.println("Translated text: " + result.getString("translatedText"));
      }
    } catch (Exception e) {
      e.printStackTrace();
    }
  }
}
