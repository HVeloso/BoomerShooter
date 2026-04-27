using UnityEngine;

public static class InterfaceTreatment
{
    /// <summary>
    /// Used to extract an interface from an object.
    /// </summary>
    /// <typeparam name="T">Interface type.</typeparam>
    /// <param name="target">Object that must have the interface.</param>
    /// <param name="extractedInterface">Interface that will return.</param>
    /// <param name="throwError">Once it is TRUE the method will throw an exception in case the interface was not found.</param>/param>
    /// <returns>Returns TRUE if the object has the correct interface.</returns>
    /// <exception cref="System.Exception"></exception>
    public static bool TryExtractInterface<T>(GameObject target, out T extractedInterface, bool throwError = true)
    {
        if (target.TryGetComponent(out T extracted))
        {
            extractedInterface = extracted;
            return true;
        }

        if (throwError)
            throw new System.Exception($"{target.name} is invalid. It is missing {extracted.GetType()} interface.");

        extractedInterface = default;
        return false;
    }
}
