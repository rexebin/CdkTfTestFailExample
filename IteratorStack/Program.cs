// See https://aka.ms/new-console-template for more information

using HashiCorp.Cdktf;

var app = new App();

_ = new IteratorStack.IteratorStack(app,"iterator-stack");

app.Synth();