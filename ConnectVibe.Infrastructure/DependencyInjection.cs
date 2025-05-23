﻿using Fliq.Application.Common.Interfaces.Authentication;
using Fliq.Application.Common.Interfaces.Helper;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Persistence.Subscriptions;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.AuthServices;
using Fliq.Application.Common.Interfaces.Services.EventServices;
using Fliq.Application.Common.Interfaces.Services.LocationServices;
using Fliq.Application.Common.Interfaces.Services.MeidaServices;
using Fliq.Application.Common.Interfaces.Services.NotificationServices;
using Fliq.Application.Common.Interfaces.Services.PaymentServices;
using Fliq.Application.Common.Interfaces.Services.SubscriptionServices;
using Fliq.Application.Explore.Common.Services;
using Fliq.Application.SchedulingServices.QuartzJobs;
using Fliq.Infrastructure.Authentication;
using Fliq.Infrastructure.Event;
using Fliq.Infrastructure.Persistence;
using Fliq.Infrastructure.Persistence.Helper;
using Fliq.Infrastructure.Persistence.Repositories;
using Fliq.Infrastructure.Persistence.Repositories.Subscriptions;
using Fliq.Infrastructure.Services;
using Fliq.Infrastructure.Services.AuthServices;
using Fliq.Infrastructure.Services.EventServices;
using Fliq.Infrastructure.Services.LocationServices;
using Fliq.Infrastructure.Services.MediaService;
using Fliq.Infrastructure.Services.NotificationServices.Email;
using Fliq.Infrastructure.Services.NotificationServices.Firebase;
using Fliq.Infrastructure.Services.PaymentServices;
using Fliq.Infrastructure.Services.SubscriptionServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using StreamChat.Clients;
using System.Text;

namespace Fliq.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configurationManager)
        {
            services.AddAuth(configurationManager);
            services.AddStream(configurationManager);
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddScoped<IPromptResponseRepository, PromptResponseRepository>();
            services.AddScoped<IPromptCategoryRepository, PromptCategoryRepository>();
            services.AddScoped<IPromptQuestionRepository, PromptQuestionRepository>();
            services.AddScoped<ISocialAuthService, SocialAuthService>();
            services.AddScoped<IOtpRepository, OtpRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            services.AddScoped<ISettingsRepository, SettingsRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<IMediaServices, MediaService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<IProfileMatchingService, ProfileMatchingService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddSingleton<ILoggerManager, LoggerManager>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IMatchProfileRepository, MatchProfileRepository>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<IRevenueCatServices, RevenueCatServices>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IEventReviewRepository, EventReviewRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IPushNotificationService, FireBaseNotificationService>();
            services.AddScoped<IEmailNotificationService, EmailNotificationService>();
            services.AddScoped<IFirebaseMessagingWrapper, FirebaseMessagingWrapper>();
            services.AddScoped<IGamesRepository, GamesRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<IStakeRepository, StakeRepository>();
            services.AddScoped<IUserFeatureActivityRepository, UserFeatureActivityRepository>();
            services.AddScoped<ISupportTicketRepository, SupportTicketRepository>();
            services.AddScoped<IBlindDateCategoryRepository, BlindDateCategoryRepository>();
            services.AddScoped<IBlindDateParticipantRepository, BlindDateParticipantRepository>();
            services.AddScoped<IBlindDateRepository, BlindDateRepository>();
            services.AddScoped<ISpeedDateParticipantRepository, SpeedDateParticipantRepository>();
            services.AddScoped<ISpeedDatingEventRepository, SpeedDatingEventRepository>();
            services.AddScoped<ISubscriptionPlanRepository, SubscriptionPlanRepository>();
            services.AddScoped<ISubscriptionPlanPriceRepository, SubscriptionPlanPriceRepository>();
            services.AddScoped<IUserSubscriptionRepository, UserSubscriptionRepository>();
            services.AddSingleton<ICustomProfileMapper, CustomProfileMapper>();
            services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            services.AddScoped<IAuditTrailRepository, AuditTrailRepository>();
            services.AddScoped<IAuditTrailService, AuditTrailService>();
            services.AddScoped<IBusinessIdentificationDocumentTypeRepository, BusinessIdentificationDocumentTypeRepository>();
            services.AddScoped<IDocumentUploadService, DocumentUploadService>();
            services.AddScoped<IBusinessIdentificationDocumentRepository, BusinessIdentificationDocumentRepository>();
            services.AddDbContext<FliqDbContext>(options =>
    options.UseSqlServer(configurationManager.GetConnectionString("FliqDbContext") ?? throw new InvalidOperationException("Connection string 'FliqDbContext' not found.")));
            return services;
        }

        public static IServiceCollection AddAuth(this IServiceCollection services, ConfigurationManager configurationManager)
        {
            services.Configure<JwtSettings>(configurationManager.GetSection(JwtSettings.SectionName));
            services.Configure<GoogleAuthSettings>(configurationManager.GetSection(GoogleAuthSettings.SectionName));
            services.Configure<FacebookAuthSettings>(configurationManager.GetSection(FacebookAuthSettings.SectionName));
            services.Configure<FaceApi>(configurationManager.GetSection(FaceApi.SectionName));
            services.Configure<EventSettings>(configurationManager.GetSection(EventSettings.SectionName));
            services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

            var jwtSettings = new JwtSettings();
            var googleAuthSettings = new GoogleAuthSettings();
            var facebookAuthSettings = new FacebookAuthSettings();
            var faceApi = new FaceApi();
            var eventSettings = new EventSettings();
            configurationManager.GetSection(JwtSettings.SectionName).Bind(jwtSettings);
            configurationManager.GetSection(GoogleAuthSettings.SectionName).Bind(googleAuthSettings);
            configurationManager.GetSection(FacebookAuthSettings.SectionName).Bind(facebookAuthSettings);
            configurationManager.GetSection(FaceApi.SectionName).Bind(faceApi);
            configurationManager.GetSection(EventSettings.SectionName).Bind(eventSettings);
            services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
                options => options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                })
                  .AddGoogle(googleOptions =>
                    {
                        googleOptions.ClientId = googleAuthSettings.ClientId;
                        googleOptions.ClientSecret = googleAuthSettings.ClientSecret;
                    });
            services.AddHttpClient("Facebook", c =>
            {
                c.BaseAddress = new Uri(configurationManager.GetValue<string>("FacebookAuthSettings:BaseUrl"));
            });
            return services;
        }

        // Register Quartz
        public static IServiceCollection AddQuartz(this IServiceCollection services, ConfigurationManager configurationManager)
        {
            services.AddQuartz(q =>
           {
               var inactivityJobKey = new JobKey("InactivityCheckJob");

               q.AddJob<InactivityCheckJob>(opts => opts.WithIdentity(inactivityJobKey));

               q.AddTrigger(opts => opts
                   .ForJob(inactivityJobKey)
                   .WithIdentity("InactivityCheckTrigger")
                   .WithCronSchedule("0 0 12 * * ?") // Runs daily at 12 PM UTC
               );

               // Register ExportUsersJob (no trigger)
               var exportJobKey = new JobKey("ExportUsersJob");
               q.AddJob<ExportUsersJob>(opts => opts.WithIdentity(exportJobKey));
           });
            return services;
        }

        public static IServiceCollection AddStream(this IServiceCollection services, ConfigurationManager configurationManager)
        {
            var key = configurationManager.GetValue<string>("StreamApi:Key");
            var secret = configurationManager.GetValue<string>("StreamApi:Secret");

            services.AddSingleton<IStreamClientFactory>(new StreamClientFactory(key, secret));
            return services;
        }

    }
}