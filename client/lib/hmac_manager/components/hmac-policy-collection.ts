import { HmacPolicy } from "./hmac-policy.js";
import { HmacScheme } from "./hmac-scheme.js";

/**
 * Represents a collection of HMAC policies and their associated schemes.
 */
export class HmacPolicyCollection {
    private readonly policyCollection: { [key: string]: HmacPolicy } = {};
    private readonly schemeCollection: { [key: string]: HmacScheme } = {};

    /**
     * Creates an instance of HmacPolicyCollection.
     * 
     * @param policies - An array of HMAC policies to be added to the collection.
     */
    constructor(policies: HmacPolicy[]) {
        for (const policy of policies) {
            this.policyCollection[policy.name] = policy;

            for (const scheme of policy.schemes) {
                this.schemeCollection[`${policy.name}:${scheme.name}`] = scheme;
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
        const matchingPolicy = this.policyCollection[policy];
        const matchingScheme = this.schemeCollection[`${policy}:${scheme}`];
        return [matchingPolicy, matchingScheme];
    }
}