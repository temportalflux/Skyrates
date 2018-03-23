using UnityEngine;

/// <summary>
/// The versioning asset - keeps track of the code's current release version.
/// Follows https://semver.org/.
/// </summary>
[CreateAssetMenu(menuName = "Data/Version")]
public class Version : ScriptableObject
{

    /// <summary>
    /// The name of the version.
    /// </summary>
    public string VersionName;

    /// <summary>
    /// The major version.
    /// Increment for major releases or when you make incompatible API changes.
    /// Pragmatically: incremented for release.
    /// </summary>
    public int Major;

    /// <summary>
    /// The minor version.
    /// Increment when you add functionality in a backwards-compatible manner.
    /// Pragmatically: incrememented for restructuring or gameplay feature updates.
    /// </summary>
    public int Minor;

    /// <summary>
    /// The patch version.
    /// Increment when you make backwards-compatible bug fixes.
    /// Pragmatically: incremented during prototyping, and when bug fixes are made in build.
    /// </summary>
    public int Patch;

    /// <summary>
    /// Returns the version as a "#.#.#" formated string.
    /// </summary>
    /// <returns></returns>
    public string GetSemantic()
    {
        return string.Format("{0}.{1}.{2}", this.Major, this.Minor, this.Patch);
    }


}
