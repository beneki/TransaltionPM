import { Component, OnInit, inject } from '@angular/core';
import { TranslatorService } from '../../core/services/translator.service';
import { Translator } from '../../core/models/translator.model';

@Component({
    selector: 'app-translators',
    templateUrl: './translators.component.html'
})
export class TranslatorsComponent implements OnInit {
    translators: Translator[] = [];
    translatorService = inject(TranslatorService);

    ngOnInit(): void {
        this.translatorService.getTranslators().subscribe(data => {
            this.translators = data;
        });
    }
}
