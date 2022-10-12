using static HaydiFunApp.Constants;
using HashidsNet;

namespace HaydiFunApp;

public static class Constants
{
  public const string PublishVersion = "HaydiFun© v0.1 Şener Demiral®";
  public const string BrowserUsrIdKey = "haydifun";
  public const string Height = "calc(100vh - 140px)";

  public static Hashids hashIds0 = new Hashids("this is my salt", 0);   // Nekadar gerekiyorsa
  public static Hashids hashIds5 = new Hashids("this is my salt", 5);
  public static Hashids hashIds8 = new Hashids("this is my salt", 8);
  public static Hashids hashIds11 = new Hashids("this is my salt", 11);
}
