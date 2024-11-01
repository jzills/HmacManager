/**
 * A class representing `HmacAuthenticationDefaults`.
 */
export class HmacAuthenticationDefaults {
    /**
     * The name of the hmac `AuthenticationScheme`. This is the scheme
     * used as the authorization header for requests signed with `IHmacManager`.
     */
    static readonly AuthenticationScheme = "Hmac";

    /**
     * The name of the `DefaultPolicy`. This is always set to "Default".
     */
    static readonly DefaultPolicy = "Default";

    /**
     * A class representing `Headers`. These are the minimum required
     * headers that are automatically added to requests signed with an `IHmacManager` implementation.
     */
    static Headers = class {
        /**
         * The `Policy` header value. Identifies the policy to sign and verify requests.
         * This is configured through an instance of `HmacManagerOptions`.
         */
        static readonly Policy = "Hmac-Policy";

        /**
         * The `Scheme` header value. Identifies the `Scheme` to sign and verify requests with. 
         * This is configured through an instance of `HmacManagerOptions`.
         */
        static readonly Scheme = "Hmac-Scheme";

        /**
         * The `Nonce` header value. This is automatically generated by
         * `IHmacManager` when signing and verifying requests.
         */
        static readonly Nonce = "Hmac-Nonce";
        
        /**
         * The `DateRequested` header value. This is automatically generated by
         * `IHmacManager` when signing and verifying requests.
         */
        static readonly DateRequested = "Hmac-Date-Requested";
    }
}