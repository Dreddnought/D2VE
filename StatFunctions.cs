namespace D2VE;

public static class StatFunctions
{
    public static bool RRD23(long mobi, long resi, long reco, long disc, long inte, long stre) => resi + reco + disc >= 23;
    public static bool MRD23(long mobi, long resi, long reco, long disc, long inte, long stre) => resi + mobi + disc >= 23;
    public static bool MRS23(long mobi, long resi, long reco, long disc, long inte, long stre) => resi + mobi + stre >= 23;
    public static bool RRI23(long mobi, long resi, long reco, long disc, long inte, long stre) => resi + reco + inte >= 23;
    public static bool RRS23(long mobi, long resi, long reco, long disc, long inte, long stre) => resi + reco + stre >= 23;
    public static bool MRI23(long mobi, long resi, long reco, long disc, long inte, long stre) => resi + mobi + inte >= 23;
    public static bool MR16(long mobi, long resi, long reco, long disc, long inte, long stre) => resi + mobi >= 16;
}
