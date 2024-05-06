import { IHeaderScheme } from "./i-header-scheme.js";
import { Header } from "./header.js";

export class HeaderScheme implements IHeaderScheme {
    readonly name: string;
    readonly collection: Header[];

    constructor(name: string) {
        this.name = name;
    }

    addHeader(name: string, claimType: string | null = null): void {
        this.collection.push(new Header(name, claimType));
    }

}