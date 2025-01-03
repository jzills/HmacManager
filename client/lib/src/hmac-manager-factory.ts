import HmacManager from "./hmac-manager";
import HmacPolicy from "./components/hmac-policy";
import HmacPolicyCollection from "./components/hmac-policy-collection";
import HmacHeaderBuilderFactory from "./builders/hmac-header-builder-factory";

/**
 * A factory class for creating instances of HmacManager based on
 * specified policies and schemes.
 */
export default class HmacManagerFactory {
    /**
     * A collection of HMAC policies used for authentication.
     */
    private readonly policies: HmacPolicyCollection;

    /**
     * Factory responsible for creating instances of HMAC header builders.
     */
    private readonly headerBuilderFactory: HmacHeaderBuilderFactory;

    /**
     * Initializes the factory with a collection of HmacPolicy objects.
     * @param policies - Array of HmacPolicy objects to manage and retrieve.
     * @param isConsolidatedHeadersEnabled - `true` if header consolidation is enabled otherwise `false`.
     */
    constructor(policies: HmacPolicy[], isConsolidatedHeadersEnabled: boolean = false) {
        this.policies = new HmacPolicyCollection(policies);
        this.headerBuilderFactory = new HmacHeaderBuilderFactory(isConsolidatedHeadersEnabled);
    }

    /**
     * Creates and returns an HmacManager instance based on the specified policy and scheme.
     * @param policy - Name of the policy to match.
     * @param scheme - Optional name of the scheme to match.
     * @returns An HmacManager instance if a matching policy is found; otherwise, null.
     */
    create(policy: string, scheme: string | null = null): HmacManager | null {
        const [matchingPolicy, matchingScheme] = this.policies.get(policy, scheme);
        if (matchingPolicy) {
            return new HmacManager(matchingPolicy, matchingScheme,
                this.headerBuilderFactory.create());
        } else {
            return null;
        }
    }
}