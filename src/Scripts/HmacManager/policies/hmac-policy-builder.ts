import { HeaderSchemeCollection } from "../headers/header-scheme-collection.js";
import { HeaderScheme } from "../headers/header-scheme.js";
import { Algorithms, ContentHashAlgorithm, SigningHashAlgorithm } from "./algorithms.js";
import { HmacPolicy } from "./hmac-policy.js";
import { KeyCredentials } from "./key-credentials.js";
import { Nonce } from "./nonce.js";

export class HmacPolicyBuilder {
    protected readonly name: string;
    protected readonly keys: KeyCredentials;
    protected readonly algorithms: Algorithms;
    protected readonly nonce: Nonce;
    protected readonly schemes: HeaderSchemeCollection;

    constructor(name: string) {
        this.name = name;
        this.keys = new KeyCredentials();
        this.nonce = new Nonce();
        this.schemes = new HeaderSchemeCollection();
    }
    
    usePublicKey(key: string): HmacPolicyBuilder {
        this.keys.publicKey = key;
        return this;
    }

    usePrivateKey(key: string): HmacPolicyBuilder {
        this.keys.privateKey = key;
        return this;
    }

    useContentHashAlgorithm(hashAlgorithm: ContentHashAlgorithm): HmacPolicyBuilder {
        this.algorithms.contentHashAlgorithm = hashAlgorithm;
        return this;
    }

    useSigningHashAlgorithm(hashAlgorithm: SigningHashAlgorithm): HmacPolicyBuilder {
        this.algorithms.signingHashAlgorithm = hashAlgorithm;
        return this;
    }

    useMemoryCache(maxAge: number): HmacPolicyBuilder {
        throw new Error("Not implemented");
    }

    useDistributedCache(maxAge: number): HmacPolicyBuilder {
        throw new Error("Not implemented")
    }

    addScheme(name: string, configureScheme: (scheme: HeaderScheme) => void): HmacPolicyBuilder {
        this.schemes.Add(name, configureScheme);   
        return this;
    }

    build(): HmacPolicy {
        return new HmacPolicy()
    }
}