import { TranslateItem } from './TranslateItem';

export interface Translate {
    translateId: string;
    langs: string[];
    fieldId: number;
    items: TranslateItem[];
    done: boolean;
    createdAt: Date;
}
