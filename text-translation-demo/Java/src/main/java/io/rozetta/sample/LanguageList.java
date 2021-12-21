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

class LanguageList {
  public static void main(String[] args) {
    JWT jwt = new JWT(API.ACCESS_KEY, API.SECRET_KEY, API.JWT_DURATION);
    try {
      String token = jwt.getToken();
      String url = API.BASE_URL + "/api/v1/languages";
      HttpRequest request = HttpRequest.newBuilder()
          .GET()
          .uri(URI.create(url))
          .header("Authorization", "Bearer " + token)
          .build();
      HttpClient httpClient = HttpClient.newBuilder()
          .version(HttpClient.Version.HTTP_2)
          .connectTimeout(Duration.ofSeconds(API.CONNECT_TIMEOUT))
          .build();
      HttpResponse<String> response = httpClient.send(request, HttpResponse.BodyHandlers.ofString());
      if ((response.statusCode() % 200) != 0) {
        throw new Exception("Unable to get language list: " + response.body());
      }
      JsonArray languages = Json.createReader(new StringReader(response.body()))
          .readObject()
          .getJsonObject("data")
          .getJsonArray("languages");
      for (JsonValue jsonValue : languages) {
        JsonObject languageObject = jsonValue.asJsonObject();
        System.out.println("Language: " + languageObject.getString("language"));
        System.out.println("Abbreviation: " + languageObject.getString("abbreviation"));
        System.out.println("Description: " + languageObject.getString("description"));
      }
    } catch (Exception e) {
      e.printStackTrace();
    }
  }
}
