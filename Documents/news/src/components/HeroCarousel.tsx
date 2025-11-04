import React, { useState, useEffect } from 'react';
import { getTopHeadlines, Article } from '../services/newsApi';
import { useTranslation } from 'react-i18next';

export default function HeroCarousel() {
  const { t } = useTranslation();
  const [articles, setArticles] = useState<Article[]>([]);
  const [currentIndex, setCurrentIndex] = useState(0);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchHeroArticles = async () => {
      try {
        const response = await getTopHeadlines();
        // Take only first 5 articles for carousel
        setArticles(response.articles.slice(0, 5));
      } catch (error) {
        console.error('Error fetching hero articles:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchHeroArticles();
  }, []);

  useEffect(() => {
    if (articles.length === 0) return;

    const interval = setInterval(() => {
      setCurrentIndex((prev) => (prev + 1) % articles.length);
    }, 5000);

    return () => clearInterval(interval);
  }, [articles.length]);

  const goToSlide = (index: number) => {
    setCurrentIndex(index);
  };

  const nextSlide = () => {
    setCurrentIndex((prev) => (prev + 1) % articles.length);
  };

  const prevSlide = () => {
    setCurrentIndex((prev) => (prev - 1 + articles.length) % articles.length);
  };

  if (loading) {
    return (
      <div className="relative h-96 bg-gradient-to-r from-blue-600 to-purple-600 rounded-2xl overflow-hidden">
        <div className="absolute inset-0 flex items-center justify-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-white"></div>
        </div>
      </div>
    );
  }

  if (articles.length === 0) {
    return null;
  }

  const currentArticle = articles[currentIndex];

  return (
    <div className="relative h-96 bg-gradient-to-r from-blue-600 to-purple-600 rounded-2xl overflow-hidden group">
      {/* Background Image with Overlay */}
      <div className="absolute inset-0">
        <img
          src={currentArticle.urlToImage || '/assets/placeholder-1.svg'}
          alt={currentArticle.title}
          className="w-full h-full object-cover opacity-40"
          onError={(e) => {
            const target = e.target as HTMLImageElement;
            target.src = '/assets/placeholder-1.svg';
          }}
        />
        <div className="absolute inset-0 bg-gradient-to-r from-black/70 via-black/50 to-transparent"></div>
      </div>

      {/* Content */}
      <div className="relative h-full flex items-center">
        <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-white">
            <div className="flex items-center gap-3 mb-4">
              <span className="px-3 py-1 bg-red-600 text-white text-xs font-semibold rounded-full uppercase tracking-wide">
                {t('news.breaking')}
              </span>
              <span className="text-sm opacity-75">
                {currentArticle.source.name}
              </span>
              <span className="text-sm opacity-75">•</span>
              <span className="text-sm opacity-75">
                {new Date(currentArticle.publishedAt).toLocaleDateString()}
              </span>
            </div>
            
            <h1 className="text-3xl md:text-4xl lg:text-5xl font-bold mb-4 leading-tight">
              {currentArticle.title}
            </h1>
            
            {currentArticle.description && (
              <p className="text-lg md:text-xl opacity-90 mb-6 line-clamp-3">
                {currentArticle.description}
              </p>
            )}
            
            <div className="flex items-center gap-4">
              <button
                onClick={() => window.open(currentArticle.url, '_blank')}
                className="px-6 py-3 bg-white text-gray-900 font-semibold rounded-lg hover:bg-gray-100 transition-colors duration-200"
              >
                {t('news.readMore')}
              </button>
              
              <button className="p-3 bg-white/20 backdrop-blur-sm rounded-lg hover:bg-white/30 transition-colors duration-200">
                <svg className="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 5a2 2 0 012-2h10a2 2 0 012 2v16l-7-3.5L5 21V5z" />
                </svg>
              </button>
              
              <button className="p-3 bg-white/20 backdrop-blur-sm rounded-lg hover:bg-white/30 transition-colors duration-200">
                <svg className="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8.684 13.342C8.886 12.938 9 12.482 9 12c0-.482-.114-.938-.316-1.342m0 2.684a3 3 0 110-2.684m9.032 4.026a9.001 9.001 0 01-7.432 0m9.032-4.026A9.001 9.001 0 0112 3c-4.474 0-8.268 3.12-9.032 7.326m0 0A9.001 9.001 0 0012 21c4.474 0 8.268-3.12 9.032-7.326" />
                </svg>
              </button>
            </div>
          </div>
        </div>
      </div>

      {/* Navigation Arrows */}
      <button
        onClick={prevSlide}
        className="absolute left-4 top-1/2 -translate-y-1/2 p-2 bg-white/20 backdrop-blur-sm rounded-lg hover:bg-white/30 transition-all duration-200 opacity-0 group-hover:opacity-100"
      >
        <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
        </svg>
      </button>
      
      <button
        onClick={nextSlide}
        className="absolute right-4 top-1/2 -translate-y-1/2 p-2 bg-white/20 backdrop-blur-sm rounded-lg hover:bg-white/30 transition-all duration-200 opacity-0 group-hover:opacity-100"
      >
        <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
        </svg>
      </button>

      {/* Dots Indicator */}
      <div className="absolute bottom-4 left-1/2 -translate-x-1/2 flex gap-2">
        {articles.map((_, index) => (
          <button
            key={index}
            onClick={() => goToSlide(index)}
            className={`w-2 h-2 rounded-full transition-all duration-300 ${
              index === currentIndex
                ? 'w-8 bg-white'
                : 'bg-white/50 hover:bg-white/75'
            }`}
          />
        ))}
      </div>
    </div>
  );
}
