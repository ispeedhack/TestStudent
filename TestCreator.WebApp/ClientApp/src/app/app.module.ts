import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { AuthService } from './services/auth.service';
import { TestResultService } from './services/test.result.service';
import { AuthInterceptor } from './services/auth.interceptor';
import { AuthResponseInterceptor } from './services/auth.response.interceptor';

import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { HomeComponent } from './components/home/home.component';
import { TestListComponent } from './components/test/test-list/test-list.component';
import { TestComponent } from './components/test/test.component';
import { TestStartComponent } from './components/test/test-start/test-start.component';
import { TestResultComponent } from './components/test/test-result/test-result.component';
import { TestEditComponent } from './components/test/test-edit/test-edit.component';
import { TestSearchComponent } from './components/test/test-search/test-search.component';
import { TestSearchResultComponent } from './components/test/test-search-result/test-search-result.component';
import { QuestionListComponent } from './components/question/question-list/question-list.component';
import { QuestionEditComponent } from './components/question/question-edit/question-edit.component';
import { AnswerListComponent } from './components/answer/answer-list/answer-list.component';
import { AnswerEditComponent } from './components/answer/answer-edit/answer-edit.component';
import { ResultListComponent } from './components/result/result-list/result-list.component';
import { ResultEditComponent } from './components/result/result-edit/result-edit.component';
import { AboutComponent } from './components/about/about.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/user/register.component';
import { PageNotFoundComponent } from './components/pagenotfound/pagenotfound.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    TestListComponent,
    TestComponent,
    TestStartComponent,
    TestResultComponent,
    TestEditComponent,
    TestSearchComponent,
    TestSearchResultComponent,
    QuestionListComponent,
    QuestionEditComponent,
    AnswerListComponent,
    AnswerEditComponent,
    ResultListComponent,
    ResultEditComponent,
    AboutComponent,
    LoginComponent,
    RegisterComponent,
    PageNotFoundComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    FontAwesomeModule ,
    RouterModule.forRoot([
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: HomeComponent },
    { path: 'test/create', component: TestEditComponent },
    { path: 'test/:id', component: TestComponent },
    { path: 'test/start/:id', component: TestStartComponent },
    { path: 'test/result/:testId', component: TestResultComponent },
    { path: 'test/edit/:id', component: TestEditComponent },
    { path: 'test/search/:text', component: TestSearchResultComponent },
    { path: 'question/edit/:id', component: QuestionEditComponent },
    { path: 'question/create/:id', component: QuestionEditComponent },
    { path: 'answer/edit/:id', component: AnswerEditComponent },
    { path: 'answer/create/:id', component: AnswerEditComponent },
    { path: 'result/edit/:id', component: ResultEditComponent },
    { path: 'result/create/:id', component: ResultEditComponent },
    { path: 'about', component: AboutComponent },
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    { path: '**', component: PageNotFoundComponent }
], { relativeLinkResolution: 'legacy' })
  ],
  providers: [
    TestResultService,
    AuthService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthResponseInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
