export class Header {
    name: string;
    claimType: string;

    constructor(name: string, claimType: string | null = null) {
        this.name = name;
        this.claimType = claimType;
    }
}