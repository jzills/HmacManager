import { HeaderSchemeCollection } from "../headers/header-scheme-collection.js";
import { Algorithms } from "./algorithms.js";
import { KeyCredentials } from "./key-credentials.js";
import { Nonce } from "./nonce.js";

export class HmacPolicy {
    readonly name: string;
    readonly keys: KeyCredentials;
    readonly algorithms: Algorithms;
    readonly nonce: Nonce;
    readonly schemes: HeaderSchemeCollection;

    constructor(
        name: string, 
        keys: KeyCredentials, 
        algorithms: Algorithms, 
        nonce: Nonce,
        schemes: HeaderSchemeCollection
    ) {
        this.name = name;
        this.keys = keys;
        this.algorithms = algorithms;
        this.nonce = nonce;
        this.schemes = schemes;
    }
}