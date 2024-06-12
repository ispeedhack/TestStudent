using System;
using System.Collections.Generic;
using NUnit.Framework;
using TestCreator.WebApp.Services;
using TestCreator.WebApp.Services.Interfaces;
using TestCreator.WebApp.ViewModels;

namespace TestCreator.Tests.Service
{
    [TestFixture]
    public class TestCalculationServiceTests
    {
        private ITestCalculationService _sut;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _sut = new TestCalculationService();
        }

        [Test]
        public void CalculateResult_NullViewModel_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _sut.CalculateResult(null));
        }

        [Test]
        public void CalculateResult_CorrectViewModelWithMultipleChoicesCorrectAnswers_ReturnsTestAttemptResultViewModel()
        {
            var viewModel = new TestAttemptViewModel
            {
                TestId = 1,
                Title = "title1",
                UserId = Guid.NewGuid().ToString(),
                TestAttemptEntries = new List<TestAttemptEntryViewModel>
                {
                    new TestAttemptEntryViewModel
                    {
                        Question = new QuestionViewModel
                        {
                            Id = 1
                        },
                        Answers = new List<TestAttemptAnswerViewModel>
                        {
                            new TestAttemptAnswerViewModel
                            {
                                Checked = true,
                                Value = 1
                            },
                            new TestAttemptAnswerViewModel
                            {
                                Checked = false,
                                Value = 0
                            },
                            new TestAttemptAnswerViewModel
                            {
                                Checked = true,
                                Value = 2
                            }
                        }
                    },
                    new TestAttemptEntryViewModel
                    {
                        Question = new QuestionViewModel
                        {
                            Id = 1
                        },
                        Answers = new List<TestAttemptAnswerViewModel>
                        {
                            new TestAttemptAnswerViewModel
                            {
                                Checked = true,
                                Value = 1
                            },
                            new TestAttemptAnswerViewModel
                            {
                                Checked = true,
                                Value = 1
                            },
                            new TestAttemptAnswerViewModel
                            {
                                Checked = true,
                                Value = 1

                            }
                        }
                    }
                }
            };

            var result = _sut.CalculateResult(viewModel);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Title, viewModel.Title);
            Assert.AreEqual(result.TestId, viewModel.TestId);
            Assert.AreEqual(result.Score, 6);
            Assert.AreEqual(result.MaximalPossibleScore, 6);
        }

        [Test]
        public void CalculateResult_CorrectViewModelWithMultipleChoicesDifferentAnswers_ReturnsTestAttemptResultViewModel()
        {
            var viewModel = new TestAttemptViewModel
            {
                TestId = 1,
                Title = "title1",
                UserId = Guid.NewGuid().ToString(),
                TestAttemptEntries = new List<TestAttemptEntryViewModel>
                {
                    new TestAttemptEntryViewModel
                    {
                        Question = new QuestionViewModel
                        {
                            Id = 1
                        },
                        Answers = new List<TestAttemptAnswerViewModel>
                        {
                            new TestAttemptAnswerViewModel
                            {
                                Checked = false,
                                Value = 1
                            },
                            new TestAttemptAnswerViewModel
                            {
                                Checked = true,
                                Value = 0
                            },
                            new TestAttemptAnswerViewModel
                            {
                                Checked = true,
                                Value = 2
                            }
                        }
                    },
                    new TestAttemptEntryViewModel
                    {
                        Question = new QuestionViewModel
                        {
                            Id = 1
                        },
                        Answers = new List<TestAttemptAnswerViewModel>
                        {
                            new TestAttemptAnswerViewModel
                            {
                                Checked = true,
                                Value = 1
                            },
                            new TestAttemptAnswerViewModel
                            {
                                Checked = false,
                                Value = 0
                            },
                            new TestAttemptAnswerViewModel
                            {
                                Checked = true,
                                Value = 1

                            }
                        }
                    }
                }
            };

            var result = _sut.CalculateResult(viewModel);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Title, viewModel.Title);
            Assert.AreEqual(result.TestId, viewModel.TestId);
            Assert.AreEqual(result.Score, 2);
            Assert.AreEqual(result.MaximalPossibleScore, 5);
        }

        [Test]
        public void CalculateResult_CorrectViewModelWithSingleChoicesDifferentAnswers_ReturnsTestAttemptResultViewModel()
        {
            var viewModel = new TestAttemptViewModel
            {
                TestId = 1,
                Title = "title1",
                UserId = Guid.NewGuid().ToString(),
                TestAttemptEntries = new List<TestAttemptEntryViewModel>
                {
                    new TestAttemptEntryViewModel
                    {
                        Question = new QuestionViewModel
                        {
                            Id = 1
                        },
                        Answers = new List<TestAttemptAnswerViewModel>
                        {
                            new TestAttemptAnswerViewModel
                            {
                                Checked = false,
                                Value = 1
                            },
                            new TestAttemptAnswerViewModel
                            {
                                Checked = true,
                                Value = 0
                            },
                            new TestAttemptAnswerViewModel
                            {
                                Checked = false,
                                Value = 0
                            }
                        }
                    },
                    new TestAttemptEntryViewModel
                    {
                        Question = new QuestionViewModel
                        {
                            Id = 1
                        },
                        Answers = new List<TestAttemptAnswerViewModel>
                        {
                            new TestAttemptAnswerViewModel
                            {
                                Checked = true,
                                Value = 1
                            },
                            new TestAttemptAnswerViewModel
                            {
                                Checked = false,
                                Value = 0
                            },
                            new TestAttemptAnswerViewModel
                            {
                                Checked = false,
                                Value = 0

                            }
                        }
                    }
                }
            };

            var result = _sut.CalculateResult(viewModel);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Title, viewModel.Title);
            Assert.AreEqual(result.TestId, viewModel.TestId);
            Assert.AreEqual(result.Score, 1);
            Assert.AreEqual(result.MaximalPossibleScore, 2);
        }

        [Test]
        public void CalculateResult_CorrectViewModelWithSingleChoicesDifferentAnswersAndNegativePoints_ReturnsTestAttemptResultViewModel()
        {
            var viewModel = new TestAttemptViewModel
            {
                TestId = 1,
                Title = "title1",
                UserId = Guid.NewGuid().ToString(),
                TestAttemptEntries = new List<TestAttemptEntryViewModel>
                {
                    new TestAttemptEntryViewModel
                    {
                        Question = new QuestionViewModel
                        {
                            Id = 1
                        },
                        Answers = new List<TestAttemptAnswerViewModel>
                        {
                            new TestAttemptAnswerViewModel
                            {
                                Checked = false,
                                Value = 1
                            },
                            new TestAttemptAnswerViewModel
                            {
                                Checked = true,
                                Value = -1
                            },
                            new TestAttemptAnswerViewModel
                            {
                                Checked = false,
                                Value = -1
                            }
                        }
                    },
                    new TestAttemptEntryViewModel
                    {
                        Question = new QuestionViewModel
                        {
                            Id = 1
                        },
                        Answers = new List<TestAttemptAnswerViewModel>
                        {
                            new TestAttemptAnswerViewModel
                            {
                                Checked = true,
                                Value = 1
                            },
                            new TestAttemptAnswerViewModel
                            {
                                Checked = false,
                                Value = -1
                            },
                            new TestAttemptAnswerViewModel
                            {
                                Checked = false,
                                Value = -1

                            }
                        }
                    }
                }
            };

            var result = _sut.CalculateResult(viewModel);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Title, viewModel.Title);
            Assert.AreEqual(result.TestId, viewModel.TestId);
            Assert.AreEqual(result.Score, 0);
            Assert.AreEqual(result.MaximalPossibleScore, 2);
        }
    }
}
