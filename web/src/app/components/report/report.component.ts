import { Component, OnInit, inject } from '@angular/core';
import { ReportService } from '../../core/services/report.service';
import { BaseChartDirective } from 'ng2-charts';
import { FormGroup, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { TranslatorsComponent } from './../translators/translators.component'

interface ProjectCountsByMonth {
    countsFirstYear: number[];
    countsSecondYear: number[];
}

@Component({
    standalone: true,
    imports: [BaseChartDirective, ReactiveFormsModule, TranslatorsComponent],
    selector: 'app-report',
    templateUrl: './report.component.html'
})
export class ReportComponent implements OnInit {
    reportService = inject(ReportService);
    myForm: FormGroup;
    isTranslatorsVisible = false;
    translatorId: string | null = null;

    public barChartOptions = {
        responsive: true,
    };
    public barChartLabels = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December']; // x-axis labels
    public barChartType = 'bar';
    public barChartLegend = true;

    barChartData: any[] | undefined = undefined;

    constructor() {
        this.myForm = new FormGroup({
            firstYear: new FormControl(2023, Validators.required),
            secondYear: new FormControl(2024, Validators.required),
            status: new FormControl(0, Validators.required),
            per: new FormControl(0, Validators.required)
        });
    }

    ngOnInit(): void {
        const status = this.myForm.get('status')?.value;

        this.myForm.get('per')?.valueChanges.subscribe(isPerTranslator => {
            console.log(isPerTranslator)
            this.isTranslatorsVisible = isPerTranslator === 0 ? false : true;
        });

        this.reportService.getProjectCountsByYears(2023, 2024, status).subscribe(data => {
            this.barChartData = [
                { data: data.countsFirstYear, label: `Statuses ${this.myForm.get('firstYear')?.value}`, backgroundColor: 'rgba(75, 192, 192, 0.6)' }, // Sales 2023 data
                { data: data.countsSecondYear, label: `Statuses ${this.myForm.get('secondYear')?.value}`, backgroundColor: 'rgba(255, 99, 132, 0.6)' }  // Sales 2024 data
            ];
        });
    }

    postSubmit(data: ProjectCountsByMonth) {
        this.barChartData = [
            { data: data.countsFirstYear, label: `Statuses ${this.myForm.get('firstYear')?.value}`, backgroundColor: 'rgba(75, 192, 192, 0.6)' }, // Sales 2023 data
            { data: data.countsSecondYear, label: `Statuses ${this.myForm.get('secondYear')?.value}`, backgroundColor: 'rgba(255, 99, 132, 0.6)' }  // Sales 2024 data
        ];
    }

    updateTrasnlator(translatorId: string): void {
        this.translatorId = translatorId;
    }

    onSubmit(): void {
        if (this.myForm.valid) {
            const status = this.myForm.get('status')?.value;
            const isPerTranslator = this.myForm.get('per')?.value || false;

            if (isPerTranslator && this.translatorId) {
                this.reportService.getProjectCountsByYearsPerTranslator(2023, 2024, status, this.translatorId).subscribe(data => {
                    this.postSubmit(data);
                });
            } else {
                this.reportService.getProjectCountsByYears(2023, 2024, status).subscribe(data => {
                    this.postSubmit(data);
                });
            }
        }
    }
}
