import HmacPolicy from "./hmac-policy";
import HmacScheme from "./hmac-scheme";

/**
 * Represents a collection of HMAC policies and their associated schemes.
 */
export default class HmacPolicyCollection {
    /** 
     * A collection of HMAC policies, where the key is a unique identifier and the value is the corresponding HmacPolicy.
     */
    private readonly policies: { [key: string]: HmacPolicy } = {};

    /** 
     * A collection of HMAC schemes, where the key is a unique identifier and the value is the corresponding HmacScheme.
     */
    private readonly schemes: { [key: string]: HmacScheme } = {};

    /**
     * Creates an instance of HmacPolicyCollection.
     * 
     * @param policies - An array of HMAC policies to be added to the collection.
     */
    constructor(policies: HmacPolicy[]) {
        for (const policy of policies) {
            this.policies[policy.name] = policy;

            for (const scheme of policy.schemes) {
                this.schemes[this.concat(policy.name, scheme.name)] = scheme;
            }
        }
    }

    /**
     * Retrieves a matching HMAC policy and scheme based on the provided names.
     * 
     * @param policy - The name of the HMAC policy to retrieve.
     * @param scheme - The name of the HMAC scheme to retrieve (optional).
     * @returns A tuple containing the matching HMAC policy and scheme, or null if not found.
     */
    get(policy: string, scheme: string | null = null): [HmacPolicy | null, HmacScheme | null] {
        const matchingPolicy = this.policies[policy];
        const matchingScheme = this.schemes[this.concat(policy, scheme)];
        return [matchingPolicy, matchingScheme];
    }

    private concat = (
        policy: string,
        scheme: string | null = null
    ): string => `${policy}:${scheme}`;
}