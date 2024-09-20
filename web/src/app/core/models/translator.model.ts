export class Translator {
    id!: string;
    firstName!: string;
    lastName!: string;
    email!: string;
    languages!: { language: { id: string, name: string, translatorLanguages: [] }, languageid: string, translatorId: string }[];
}
