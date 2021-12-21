package io.rozetta.sample;

final class API {
  public static final String BASE_URL = "https://translate.rozetta-api.io";

  /**
   * Signans API connection timeout (30 seconds).
   */
  public static final int CONNECT_TIMEOUT = 30;

  public static final String ACCESS_KEY = "YOUR_ACCESS_KEY";

  public static final String SECRET_KEY = "YOUR_SECRET_KEY";

  /**
   * Valid period of the retrieved JWT (60 seconds).
   */
  public static final int JWT_DURATION = 60;

  private API() {
  }
}
