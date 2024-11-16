import { HmacAuthenticationDefaults } from "../hmac-authentication-defaults.js";
import { HmacHeaderCollection } from "./hmac-options-header-builder.js";

/**
 * A builder class to facilitate the construction of HMAC authentication headers.
 * This class allows for the fluent construction of HMAC headers, with each header 
 * being set individually before building the final header collection.
 * 
 * Example usage:
 * ```typescript
 * const builder = new HmacHeaderBuilder();
 * const headers = builder
 *     .withAuthorization('signature123')
 *     .withPolicy('policyABC')
 *     .withScheme('HMAC')
 *     .withNonce('nonce123')
 *     .withDateRequested('2024-11-15')
 *     .build();
 * ```
 */
export default class HmacHeaderBuilder {
    /**
     * The collection of header values used in HMAC authentication.
     * These values are initially set to `null` and can be updated via the
     * provided methods.
     */
    protected readonly HeaderValues: HmacHeaderCollection = {
        [HmacAuthenticationDefaults.Headers.Authorization]: null,
        [HmacAuthenticationDefaults.Headers.Policy]: null,
        [HmacAuthenticationDefaults.Headers.Scheme]: null,
        [HmacAuthenticationDefaults.Headers.Nonce]: null,
        [HmacAuthenticationDefaults.Headers.DateRequested]: null
    };

    /**
     * Retrieves the header values that are not null.
     * This method filters out any headers that have `null` as their value.
     * 
     * @returns A new object containing only the header values that are set.
     */
    protected get NonEmptyHeaderValues(): HmacHeaderCollection {
        const nonEmptyHeaderValues: HmacHeaderCollection = {};
        for (const [key, value] of Object.entries(this.HeaderValues)) {
            if (value !== null) {
                nonEmptyHeaderValues[key as HmacAuthenticationDefaults.Headers] = value;
            }
        }
        return nonEmptyHeaderValues;
    }

    /**
     * Sets the `Authorization` header with the provided signature.
     * The signature is prefixed with the default authentication scheme.
     * 
     * @param signature The HMAC signature to be included in the `Authorization` header.
     * @returns The builder instance for fluent chaining.
     */
    withAuthorization(signature: string): HmacHeaderBuilder {
        this.HeaderValues[HmacAuthenticationDefaults.Headers.Authorization] = `${HmacAuthenticationDefaults.AuthenticationScheme} ${signature}`;
        return this;
    }

    /**
     * Sets the `Policy` header with the provided policy value.
     * 
     * @param policy The policy to be included in the `Policy` header.
     * @returns The builder instance for fluent chaining.
     */
    withPolicy(policy: string): HmacHeaderBuilder {
        this.HeaderValues[HmacAuthenticationDefaults.Headers.Policy] = policy;
        return this;
    }

    /**
     * Sets the `Scheme` header with the provided scheme value.
     * 
     * @param scheme The scheme to be included in the `Scheme` header.
     * @returns The builder instance for fluent chaining.
     */
    withScheme(scheme: string | null): HmacHeaderBuilder {
        this.HeaderValues[HmacAuthenticationDefaults.Headers.Scheme] = scheme;
        return this;
    }

    /**
     * Sets the `Nonce` header with the provided nonce value.
     * 
     * @param nonce The nonce to be included in the `Nonce` header.
     * @returns The builder instance for fluent chaining.
     */
    withNonce(nonce: string): HmacHeaderBuilder {
        this.HeaderValues[HmacAuthenticationDefaults.Headers.Nonce] = nonce;
        return this;
    }

    /**
     * Sets the `DateRequested` header with the provided date value.
     * 
     * @param dateRequested The date requested to be included in the `DateRequested` header.
     * @returns The builder instance for fluent chaining.
     */
    withDateRequested(dateRequested: Date): HmacHeaderBuilder {
        this.HeaderValues[HmacAuthenticationDefaults.Headers.DateRequested] = dateRequested.getTime().toString();
        return this;
    }

    /**
     * Builds and returns the final header collection, excluding any headers with `null` values.
     * 
     * @returns A collection of headers with only non-null values.
     */
    build = (): HmacHeaderCollection => this.NonEmptyHeaderValues;
}
