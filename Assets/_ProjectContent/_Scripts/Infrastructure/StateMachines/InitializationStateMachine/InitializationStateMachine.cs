﻿using Infrastructure.Factories;
using Infrastructure.Services.Logging;
using Infrastructure.StateMachines.InitializationStateMachine.States;
using Infrastructure.StateMachines.StateMachine;
using JetBrains.Annotations;

namespace Infrastructure.StateMachines.InitializationStateMachine
{
    [UsedImplicitly]
    public class InitializationStateMachine : BaseStateMachine
    {
        protected override LogTag LogTag => LogTag.InitializationStateMachine;

        public InitializationStateMachine(IStatesFactory statesFactory, IConditionalLoggingService conditionalLoggingService) : base(conditionalLoggingService)
        {
            RegisterState(statesFactory.Create<InitializeDefaultConfigState>(this));
            RegisterState(statesFactory.Create<InitializeUnityServicesState>(this));
#if DEV
            RegisterState(statesFactory.Create<InitializeDebugToolsState>(this));
#endif
            RegisterState(statesFactory.Create<InitializeSaveServiceState>(this));
            RegisterState(statesFactory.Create<InitializePrivacyPolicyState>(this));
            RegisterState(statesFactory.Create<InitializationFinalizerState>(this));
        }
    }
}